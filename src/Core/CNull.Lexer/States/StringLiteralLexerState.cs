using System.Text;
using CNull.Common.Configuration;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors.Compilation;
using CNull.Lexer.Constants;
using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// Represents a state in which string literal is built.
    /// </summary>
    /// <param name="source"></param>
    public class StringLiteralLexerState(ICodeSource source, IErrorHandler errorHandler, ICompilerConfiguration configuration) 
        : LexerState(source, errorHandler, configuration)
    {
        private readonly StringBuilder _builder = new();

        public override bool TryBuildToken(out Token token)
        {
            if (Source.CurrentCharacter != '"')
                return TokenFailed(out token, new InvalidTokenStartCharacter(TokenPosition), false);

            Source.MoveToNext();
            var isEscapeSequence = false;

            var lengthCounter = 0;
            while (Source.CurrentCharacter != '"' || (isEscapeSequence && Source.CurrentCharacter == '"'))
            {
                if (++lengthCounter > configuration.MaxStringLiteralLength)
                    return TokenFailed(out token,
                        new InvalidTokenLengthError(TokenPosition, configuration.MaxStringLiteralLength), false);

                if (!Source.CurrentCharacter.HasValue || Source.IsCurrentCharacterNewLine)
                    return TokenFailed(out token, new LineBreakedTextLiteralError(TokenPosition));

                if (isEscapeSequence)
                {
                    char sequenceCharacter = default;

                    if(!TokenHelpers.TryBuildEscapeSequence(Source.CurrentCharacter.Value, ref sequenceCharacter))
                        return TokenFailed(out token, new InvalidEscapeSequenceError(TokenPosition));

                    _builder.Append(sequenceCharacter);
                    isEscapeSequence = false;
                }
                else
                {
                    if (Source.CurrentCharacter == '\\')
                        isEscapeSequence = true;
                    else _builder.Append(Source.CurrentCharacter);
                }

                Source.MoveToNext();
            } 

            Source.MoveToNext();
            token = new Token<string>(_builder.ToString(), TokenType.StringLiteral, TokenPosition);
            return true;
        }
    }
}
