using CNull.Common;
using CNull.Lexer.Constants;

namespace CNull.Lexer.Tests.Data
{
    public class IdentifiersData : TheoryData<string, bool, Token>
    {
        public IdentifiersData()
        {
            Add("", false, Token.Unknown(Position.FirstCharacter));
            Add("abcde-", true, new Token<string>("abcde", TokenType.Identifier, Position.FirstCharacter));
            Add("ABCDE/", true, new Token<string>("ABCDE", TokenType.Identifier, Position.FirstCharacter));
            Add("abc_DEF%", true, new Token<string>("abc_DEF", TokenType.Identifier, Position.FirstCharacter));
            Add("abc_DEF*", true, new Token<string>("abc_DEF", TokenType.Identifier, Position.FirstCharacter));
            Add("_abcde", true, new Token<string>("_abcde", TokenType.Identifier, Position.FirstCharacter));
            Add("_____", true, new Token<string>("_____", TokenType.Identifier, Position.FirstCharacter));
            Add("_____1-", true, new Token<string>("_____1", TokenType.Identifier, Position.FirstCharacter));
            Add("_abcde_", true, new Token<string>("_abcde_", TokenType.Identifier, Position.FirstCharacter));
            Add("sampleToken1234567890 = 1;", true, new Token<string>("sampleToken1234567890", TokenType.Identifier, Position.FirstCharacter));
            Add("sampleToken1234567890_a__.SomeFurtherThing()", true, new Token<string>("sampleToken1234567890_a__", TokenType.Identifier, Position.FirstCharacter));
            Add("1token", false, Token.Unknown(Position.FirstCharacter));
            Add(".token", false, Token.Unknown(Position.FirstCharacter));
            Add(string.Join("", Enumerable.Repeat("a", 1000)), false, Token.Unknown(Position.FirstCharacter));
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
                Add($"{literalToken}(some further things)", true, new Token(TokenHelpers.KeywordsToTokenTypes[literalToken], Position.FirstCharacter));

            Add(" .??", false, Token.Unknown(Position.FirstCharacter));
        }
    }
}
