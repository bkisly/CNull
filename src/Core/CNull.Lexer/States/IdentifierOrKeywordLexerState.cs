using System.Text;
using CNull.Lexer.Constants;
using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// State in which identifier or keyword is built.
    /// </summary>
    public class IdentifierOrKeywordLexerState(ICodeSource source) : LexerState(source)
    {
        private readonly StringBuilder _tokenBuilder = new();
        private bool FirstCharacterBuilt => _tokenBuilder.Length > 0;

        public override bool TryBuildToken(out Token token)
        {
            if (!CurrentCharacter.HasValue || CurrentCharacter.IsTokenTerminator())
                return TokenFailed(out token);

            do
            {
                if (IsValidCharacter(CurrentCharacter.Value))
                    _tokenBuilder.Append(CurrentCharacter.Value);
                else return TokenFailed(out token);

                Source.MoveToNext();
                // @TODO: handle excessively long tokens
            } 
            while (!CurrentCharacter.IsTokenTerminator());

            var literalToken = _tokenBuilder.ToString();
            token = TokenHelpers.KeywordsToTokenTypes.TryGetValue(literalToken, out var tokenType) 
                ? new Token(tokenType) 
                : new Token<string>(literalToken, TokenType.Identifier);

            return true;
        }

        private static bool IsValidFirstCharacter(char character)
            => char.IsLetter(character) || character == '_';

        private bool IsValidCharacter(char character)
            => !FirstCharacterBuilt
                ? IsValidFirstCharacter(character)
                : IsValidFirstCharacter(character) || char.IsAsciiDigit(character);
    }
}
