using System.Text;
using CNull.ErrorHandler.Errors.Compilation;
using CNull.Lexer.Constants;
using CNull.Lexer.ServicesContainers;

namespace CNull.Lexer.States
{
    /// <summary>
    /// State in which identifier or keyword is built.
    /// </summary>
    public class IdentifierOrKeywordLexerState(ILexerStateServicesContainer servicesContainer) : LexerState(servicesContainer)
    {
        private readonly StringBuilder _tokenBuilder = new();
        private bool FirstCharacterBuilt => _tokenBuilder.Length > 0;

        public override Token BuildToken()
        {
            if (!CurrentCharacter.HasValue || CurrentCharacter.IsTokenTerminator())
                return TokenFailed(new InvalidTokenStartCharacter(TokenPosition), false);

            var lengthCounter = 0;
            do
            {
                if (++lengthCounter > Configuration.MaxIdentifierLength)
                    return TokenFailed(new InvalidTokenLengthError(TokenPosition, Configuration.MaxIdentifierLength), false);

                if (IsValidCharacter(CurrentCharacter.Value))
                    _tokenBuilder.Append(CurrentCharacter.Value);
                else return TokenFailed(new InvalidIdentifierError(TokenPosition));

                Source.MoveToNext();
            } 
            while (!CurrentCharacter.IsTokenTerminator());

            var literalToken = _tokenBuilder.ToString();
            var token = TokenHelpers.KeywordsToTokenTypes.TryGetValue(literalToken, out var tokenType) 
                ? new Token(tokenType, TokenPosition) 
                : new Token<string>(literalToken, TokenType.Identifier, TokenPosition);

            return token;
        }

        private static bool IsValidFirstCharacter(char character)
            => char.IsLetter(character) || character == '_';

        private bool IsValidCharacter(char character)
            => !FirstCharacterBuilt
                ? IsValidFirstCharacter(character)
                : IsValidFirstCharacter(character) || char.IsAsciiDigit(character);
    }
}
