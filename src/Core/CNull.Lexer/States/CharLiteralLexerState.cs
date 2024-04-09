using CNull.Common.Configuration;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors.Compilation;
using CNull.Lexer.Constants;
using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// Represents a state in which char literal is built.
    /// </summary>
    /// <param name="source"></param>
    public class CharLiteralLexerState(ICodeSource source, IErrorHandler errorHandler, ICompilerConfiguration configuration) 
        : LexerState(source, errorHandler, configuration)
    {
        public override bool TryBuildToken(out Token token)
        {
            if (Source.CurrentCharacter != '\'')
                return TokenFailed(out token, new InvalidTokenStartCharacter(TokenPosition), false);

            Source.MoveToNext();

            var isEscapeSequence = false;
            char literalContent = default;

            switch (Source.CurrentCharacter)
            {
                case '\'' or null:
                    return TokenFailed(out token, new EmptyCharLiteralError(TokenPosition));
                case '\\':
                    isEscapeSequence = true;
                    Source.MoveToNext();
                    break;
                default:
                    if (Source.IsCurrentCharacterNewLine)
                        return TokenFailed(out token, new LineBreakedTextLiteralError(TokenPosition));
                    break;
            }

            if (!isEscapeSequence)
                literalContent = Source.CurrentCharacter.Value;
            else if (!TokenHelpers.TryBuildEscapeSequence(Source.CurrentCharacter.Value, ref literalContent))
                return TokenFailed(out token, new InvalidEscapeSequenceError(TokenPosition));

            Source.MoveToNext();

            if (Source.CurrentCharacter != '\'')
                return TokenFailed(out token, new UnterminatedCharLiteral(TokenPosition));

            Source.MoveToNext();
            token = new Token<char>(literalContent, TokenType.CharLiteral, TokenPosition);
            return true;
        }
    }
}
