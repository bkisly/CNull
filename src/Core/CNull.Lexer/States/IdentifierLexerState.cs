using System.Text;
using CNull.Lexer.Constants;
using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// State in which identifier or keyword is built.
    /// </summary>
    public class IdentifierLexerState(ICodeSource source) : ILexerState
    {
        private readonly StringBuilder _tokenBuilder = new();

        public bool TryBuildToken(out Token token)
        {
            if (source.CurrentCharacter == null)
                return TokenFailed(out token);

            var tokenBuilt = false;
            char? previousCharacter = null;

            while (!tokenBuilt)
            {
                var currentCharacter = source.CurrentCharacter.Value;

                if (TokenHelpers.IsTokenTerminator(currentCharacter))
                    tokenBuilt = true;
                else
                {
                    if ((char.IsLetter(currentCharacter) || currentCharacter == '_') 
                       && (!previousCharacter.HasValue || !char.IsDigit(previousCharacter.Value)))
                        _tokenBuilder.Append(currentCharacter);
                    else if (char.IsAsciiDigit(currentCharacter))
                        _tokenBuilder.Append(currentCharacter);
                    else return TokenFailed(out token);
                }

                previousCharacter = currentCharacter;
                source.MoveToNext();
            }

            var literalToken = _tokenBuilder.ToString();
            token = TokenHelpers.KeywordsToTokenTypes.TryGetValue(literalToken, out var tokenType) 
                ? new Token(tokenType) 
                : new Token<string>(literalToken, TokenType.Identifier);

            return true;
        }

        private bool TokenFailed(out Token token)
        {
            token = new Token(TokenType.End);
            source.MoveToNext();
            return false;
        }
    }
}
