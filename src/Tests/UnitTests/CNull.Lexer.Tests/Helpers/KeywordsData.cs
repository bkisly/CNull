using CNull.Lexer.Constants;

namespace CNull.Lexer.Tests.Helpers
{
    public class KeywordsData : TheoryData<string, bool, Token>
    {
        public KeywordsData()
        {
            foreach (var literalToken in TokenHelpers.KeywordsToTokenTypes.Keys)
                Add($"{literalToken}(some further things)", true, new Token(TokenHelpers.KeywordsToTokenTypes[literalToken]));
        }
    }
}
