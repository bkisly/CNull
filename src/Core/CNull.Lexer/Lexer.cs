using System.Text;
using CNull.Common;
using CNull.Common.Configuration;
using CNull.Common.Extensions;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors;
using CNull.Lexer.Constants;
using CNull.Lexer.Errors;
using CNull.Source;

namespace CNull.Lexer
{
    public class Lexer : IRawLexer
    {
        private readonly ICodeSource _source;
        private readonly IErrorHandler _errorHandler;
        private readonly ICNullConfiguration _config;

        private readonly Dictionary<Func<char, bool>, Func<Token>> _predicateToTokenFactoryMap;
        private char? CurrentCharacter => _source.CurrentCharacter;

        public Token? LastToken { get; private set; }

        public Lexer(ICodeSource source, IErrorHandler errorHandler, ICNullConfiguration config)
        {
            _source = source;
            _errorHandler = errorHandler;
            _config = config;
            _source.SourceInitialized += (_, _) => LastToken = null;

            _predicateToTokenFactoryMap = new Dictionary<Func<char, bool>, Func<Token>>
            {
                { c => char.IsLetter(c) || c == '_', BuildIdentifierOrKeyword },
                { char.IsAsciiDigit, BuildNumericValue },
                { c => c.IsOperatorCandidate(), BuildOperatorOrComment },
                { c => c == '"', BuildStringLiteral },
                { c => c == '\'', BuildCharLiteral }
            };
        }

        public Token GetNextToken()
        {
            if (!TrySkipWhitespace())
                return Token.Unknown(_source.Position);

            var factory = GetTokenFactory(_source.CurrentCharacter);
            if (factory == null)
                return CreateToken(new Token(TokenType.End, _source.Position));

            var token = factory.Invoke();
            return CreateToken(token);
        }

        #region Common methods

        private Token CreateToken(Token token)
        {
            LastToken = token;
            return token;
        }

        private bool TrySkipWhitespace()
        {
            if (_source.CurrentCharacter == null)
                return true;

            var lengthCounter = 0;
            while (_source.CurrentCharacter.HasValue && char.IsWhiteSpace(_source.CurrentCharacter.Value))
            {
                if (++lengthCounter > _config.MaxWhitespaceLength)
                {
                    _errorHandler.RaiseCompilationError(new InvalidTokenLengthError(_source.Position, _config.MaxWhitespaceLength));
                    return false;
                }

                _source.MoveToNext();
            }

            return true;
        }

        private Func<Token>? GetTokenFactory(char? firstCharacter)
        {
            if (firstCharacter == null)
                return null;

            return _predicateToTokenFactoryMap.Where(kvp => kvp.Key.Invoke(firstCharacter.Value))
                .Select(kvp => kvp.Value)
                .FirstOrDefault();
        }

        protected Token TokenFailed(ICompilationError error, Position position, bool shouldSkipToken = true)
        {
            if (shouldSkipToken)
                SkipToken();

            _errorHandler.RaiseCompilationError(error);
            return Token.Unknown(position);
        }

        protected virtual void SkipToken()
        {
            var counter = 0;
            while (_source.CurrentCharacter.CanContinueToken())
            {
                if (++counter > _config.MaxTokenLength)
                    _errorHandler.RaiseCompilationError(new InvalidTokenLengthError(_source.Position, _config.MaxTokenLength));

                _source.MoveToNext();
            }
        }

        #endregion

        #region Token factories

        private Token BuildIdentifierOrKeyword()
        {
            var builder = new StringBuilder();
            var position = _source.Position;

            while (CurrentCharacter.CanContinueToken())
            {
                if (builder.Length >= _config.MaxIdentifierLength)
                    return TokenFailed(new InvalidTokenLengthError(_source.Position, _config.MaxIdentifierLength), position, shouldSkipToken: false);

                if (IsValidCharacter(CurrentCharacter, builder.Length))
                    builder.Append(CurrentCharacter!.Value);
                else return TokenFailed(new InvalidIdentifierError(position), position);

                _source.MoveToNext();
            }

            var literalToken = builder.ToString();
            var isKeyword = TokenHelpers.KeywordsToTokenTypes.TryGetValue(literalToken, out var tokenType);

            if (!isKeyword)
                return new Token<string>(builder.ToString(), TokenType.Identifier, position);

            return tokenType switch
            {
                TokenType.TrueKeyword or TokenType.FalseKeyword => new Token<bool>(tokenType == TokenType.TrueKeyword, tokenType, position),
                TokenType.NullKeyword => new Token<object?>(null, TokenType.NullKeyword, position),
                _ => new Token(tokenType, position)
            };
        }

        private static bool IsValidCharacter(char? character, int currentLength)
        {
            var validationBase = character.HasValue && (char.IsLetter(character.Value) || character.Value == '_');
            return currentLength == 0 ? validationBase : validationBase || character.IsAsciiDigit();
        }

        private Token BuildNumericValue()
        {
            long integerPart = 0, fractionPart = 0;
            var maxFractionDigits = long.MaxValue.Length();
            var position = _source.Position;

            if (CurrentCharacter is '0')
            {
                _source.MoveToNext();
                if (CurrentCharacter.IsAsciiDigit())
                    return TokenFailed(new PrefixedZeroError(position), position);
            }
            else if (!TryBuildNumberPart(ref integerPart, out _, int.MaxValue, _config.MaxTokenLength))
                return TokenFailed(new NumericValueOverflowError(position), position);

            if (CurrentCharacter is not '.')
                return new Token<int>((int)integerPart, TokenType.IntegerLiteral, position);

            _source.MoveToNext();

            if (!TryBuildNumberPart(ref fractionPart, out var fractionLength, maxDigits: maxFractionDigits))
                return TokenFailed(new NumericValueOverflowError(_source.Position), position);

            var fractionValue = fractionLength / (decimal)Math.Pow(10, fractionLength);
            return new Token<float>(integerPart + (float)fractionValue, TokenType.FloatLiteral, position);
        }

        private bool TryBuildNumberPart(ref long partValue, out int partLength, long maxValue = long.MaxValue, int maxDigits = int.MaxValue)
        {
            partLength = 0;

            while (CurrentCharacter.IsAsciiDigit())
            {
                var digitValue = CurrentCharacter!.Value - '0';
                if (partLength > maxDigits || partValue > (maxValue - digitValue) / 10)
                    return false;

                partValue = partValue * 10 + digitValue;
                partLength++;

                _source.MoveToNext();
            }

            return true;
        }

        private Token BuildOperatorOrComment()
        {
            var position = _source.Position;
            var builtOperator = CurrentCharacter.ToString() ?? string.Empty;
            _source.MoveToNext();

            var doubleOperatorCandidate = builtOperator + CurrentCharacter;
            if (TokenHelpers.OperatorsAndPunctors.Contains(doubleOperatorCandidate))
            {
                _source.MoveToNext();
                return new Token(TokenHelpers.OperatorsToTokenTypes[doubleOperatorCandidate], position);
            }

            if (doubleOperatorCandidate != "//")
                return TokenHelpers.OperatorsAndPunctors.Contains(builtOperator)
                    ? new Token(TokenHelpers.OperatorsToTokenTypes[builtOperator], position)
                    : TokenFailed(new UnknownOperatorError(position), position);

            _source.MoveToNext();
            return BuildComment(position);
        }

        private Token BuildComment(Position position)
        {
            var builder = new StringBuilder();

            while (CurrentCharacter.IsWhiteSpace() && !_source.IsCurrentCharacterNewLine)
                _source.MoveToNext();

            while (_source is { IsCurrentCharacterNewLine: false, CurrentCharacter: not null })
            {
                if (builder.Length >= _config.MaxCommentLength)
                    return TokenFailed(new InvalidTokenLengthError(_source.Position, _config.MaxCommentLength), position, false);

                builder.Append(CurrentCharacter);
                _source.MoveToNext();
            }

            _source.MoveToNext();
            return new Token<string>(builder.ToString(), TokenType.Comment, position);
        }

        private Token BuildStringLiteral()
        {
            var builder = new StringBuilder();
            var position = _source.Position;

            _source.MoveToNext();
            var isEscapeSequence = false;

            while (CurrentCharacter != '"' || (isEscapeSequence && CurrentCharacter == '"'))
            {
                if (builder.Length >= _config.MaxStringLiteralLength)
                    return TokenFailed(new InvalidTokenLengthError(_source.Position, _config.MaxStringLiteralLength), position, false);

                if (!CurrentCharacter.HasValue || _source.IsCurrentCharacterNewLine)
                    return TokenFailed(new LineBreakedTextLiteralError(_source.Position), position);

                if (isEscapeSequence)
                {
                    char sequenceCharacter = default;

                    if (!TokenHelpers.TryBuildEscapeSequence(CurrentCharacter.Value, ref sequenceCharacter))
                        return TokenFailed(new InvalidEscapeSequenceError(_source.Position), position);

                    builder.Append(sequenceCharacter);
                    isEscapeSequence = false;
                }
                else
                {
                    if (CurrentCharacter == '\\')
                        isEscapeSequence = true;
                    else builder.Append(CurrentCharacter);
                }

                _source.MoveToNext();
            }

            _source.MoveToNext();
            return new Token<string>(builder.ToString(), TokenType.StringLiteral, position);
        }

        private Token BuildCharLiteral()
        {
            var position = _source.Position;
            _source.MoveToNext();

            var isEscapeSequence = false;
            char literalContent = default;

            switch (CurrentCharacter)
            {
                case '\'' or null:
                    return TokenFailed(new EmptyCharLiteralError(position), position);
                case '\\':
                    isEscapeSequence = true;
                    _source.MoveToNext();
                    break;
                default:
                    if (_source.IsCurrentCharacterNewLine)
                        return TokenFailed(new LineBreakedTextLiteralError(_source.Position), position);
                    break;
            }

            if (!isEscapeSequence)
                literalContent = CurrentCharacter.Value;
            else if (!TokenHelpers.TryBuildEscapeSequence(CurrentCharacter.Value, ref literalContent))
                return TokenFailed(new InvalidEscapeSequenceError(_source.Position), position);

            _source.MoveToNext();

            if (CurrentCharacter != '\'')
                return TokenFailed(new UnterminatedCharLiteralError(_source.Position), position);

            _source.MoveToNext();
            return new Token<char>(literalContent, TokenType.CharLiteral, position);
        }

        #endregion
    }
}
