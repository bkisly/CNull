using System.Text;
using CNull.Common;
using CNull.Common.Configuration;
using CNull.Common.Extensions;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors;
using CNull.ErrorHandler.Errors.Compilation;
using CNull.Lexer.Constants;
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

        protected Token TokenFailed(ICompilationError error, bool shouldSkipToken = true)
        {
            if (shouldSkipToken)
                SkipToken();

            _errorHandler.RaiseCompilationError(error);
            return Token.Unknown(error.Position);
        }

        protected virtual void SkipToken()
        {
            var counter = 0;
            while (!_source.CurrentCharacter.CanContinueToken())
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
                    return TokenFailed(new InvalidTokenLengthError(_source.Position, _config.MaxIdentifierLength), shouldSkipToken: false);

                if (IsValidCharacter(CurrentCharacter, builder.Length))
                    builder.Append(CurrentCharacter!.Value);
                else return TokenFailed(new InvalidIdentifierError(position));

                _source.MoveToNext();
            }

            if (builder.Length == 0)
                return TokenFailed(new InvalidTokenStartCharacter(position));

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
            return currentLength > 0 ? validationBase : validationBase || character.IsAsciiDigit();
        }

        private Token BuildNumericValue()
        {
            throw new NotImplementedException();
        }

        private Token BuildOperatorOrComment()
        {
            throw new NotImplementedException();
        }

        private Token BuildStringLiteral()
        {
            throw new NotImplementedException();
        }

        private Token BuildCharLiteral()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
