using CNull.ErrorHandler.Errors.Compilation;
using CNull.Lexer.Constants;
using CNull.Lexer.ServicesContainers;

namespace CNull.Lexer.States
{
    /// <summary>
    /// Represents a state in which char literal is built.
    /// </summary>
    public class CharLiteralLexerState(ILexerStateServicesContainer servicesContainer) : LexerState(servicesContainer)
    {
        public override Token BuildToken()
        {
            if (Source.CurrentCharacter != '\'')
                return TokenFailed(new InvalidTokenStartCharacter(TokenPosition), false);

            Source.MoveToNext();

            var isEscapeSequence = false;
            char literalContent = default;

            switch (Source.CurrentCharacter)
            {
                case '\'' or null:
                    return TokenFailed(new EmptyCharLiteralError(TokenPosition));
                case '\\':
                    isEscapeSequence = true;
                    Source.MoveToNext();
                    break;
                default:
                    if (Source.IsCurrentCharacterNewLine)
                        return TokenFailed(new LineBreakedTextLiteralError(TokenPosition));
                    break;
            }

            if (!isEscapeSequence)
                literalContent = Source.CurrentCharacter.Value;
            else if (!TokenHelpers.TryBuildEscapeSequence(Source.CurrentCharacter.Value, ref literalContent))
                return TokenFailed( new InvalidEscapeSequenceError(TokenPosition));

            Source.MoveToNext();

            if (Source.CurrentCharacter != '\'')
                return TokenFailed(new UnterminatedCharLiteral(TokenPosition));

            Source.MoveToNext();
            return new Token<char>(literalContent, TokenType.CharLiteral, TokenPosition);
        }
    }
}
