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
        private char? CurrentCharacter => source.CurrentCharacter;

        public override bool TryBuildToken(out Token token)
        {
            if (!CurrentCharacter.HasValue)
                return TokenFailed(out token);

            var correctToken = true;

            while (!TokenHelpers.IsTokenTerminator(CurrentCharacter) && correctToken)
            {
                if (IsValidCharacter(CurrentCharacter.Value))
                    _tokenBuilder.Append(CurrentCharacter.Value);
                else correctToken = false;

                Source.MoveToNext();
                // @TODO: handle excessively long tokens
            }

            if (!correctToken)
                return TokenFailed(out token);

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
                : IsValidFirstCharacter(character) || char.IsDigit(character);
    }
}
