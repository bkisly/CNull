using CNull.Lexer.Constants;

namespace CNull.Lexer.Tests.Helpers
{
    public class IdentifiersData : TheoryData<string, bool, Token>
    {
        public IdentifiersData()
        {
            Add("", false, new Token(TokenType.End));
            Add("abcde-", true, new Token<string>("abcde", TokenType.Identifier));
            Add("ABCDE/", true, new Token<string>("ABCDE", TokenType.Identifier));
            Add("abc_DEF%", true, new Token<string>("abc_DEF", TokenType.Identifier));
            Add("abc_DEF*", true, new Token<string>("abc_DEF", TokenType.Identifier));
            Add("_abcde", true, new Token<string>("_abcde", TokenType.Identifier));
            Add("_____", true, new Token<string>("_____", TokenType.Identifier));
            Add("_____1-", true, new Token<string>("_____1", TokenType.Identifier));
            Add("_abcde_", true, new Token<string>("_abcde_", TokenType.Identifier));
            Add("sampleToken1234567890 = 1;", true, new Token<string>("sampleToken1234567890", TokenType.Identifier));
            Add("sampleToken1234567890_a__.SomeFurtherThing()", true, new Token<string>("sampleToken1234567890_a__", TokenType.Identifier));
            Add("1token", false, new Token(TokenType.End));
        }
    }
}
