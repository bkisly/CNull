using CNull.Lexer.Constants;

namespace CNull.Lexer.Tests.Helpers
{
    public class IdentifiersData : TheoryData<string, bool, Token>
    {
        public IdentifiersData()
        {
            Add("", false, Token.Unknown());
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
            Add("1token", false, Token.Unknown());
            Add(".token", false, Token.Unknown());
            Add(string.Join("", Enumerable.Repeat("a", 1000)), false, Token.Unknown());
        }
    }

    public class IdentifiersLastCharacterData : TheoryData<string, char?>
    {
        public IdentifiersLastCharacterData()
        {
            Add("", null);
            Add("abcde-", '-');
            Add("ABCDE/", '/');
            Add("abc_DEF%", '%');
            Add("abc_*DEF*", '*');
            Add("_abcde", null);
            Add("__;___", ';');
            Add("_____1-", '-');
            Add("_abcde_", null);
            Add("sampleToken1234567890 = 1;", ' ');
            Add("sampleToken1234567890_a__.SomeFurtherThing()", '.');
            Add("sampleToken\n1234567890_a__.SomeFurtherThing()", '\n');
            Add("1token();", '(');
        }
    }

    public class KeywordsData : TheoryData<string, bool, Token>
    {
        public KeywordsData()
        {
            foreach (var literalToken in TokenHelpers.KeywordsToTokenTypes.Keys)
                Add($"{literalToken}(some further things)", true, new Token(TokenHelpers.KeywordsToTokenTypes[literalToken]));

            Add(" .??", false, Token.Unknown());
        }
    }
}
