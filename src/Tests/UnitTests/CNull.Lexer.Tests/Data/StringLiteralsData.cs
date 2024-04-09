using CNull.Common;
using CNull.Lexer.Constants;

namespace CNull.Lexer.Tests.Data
{
    public class StringLiteralsData : TheoryData<string, bool, Token>
    {
        public StringLiteralsData()
        {
            Add("\"sample literal\" next things", true, new Token<string>("sample literal", TokenType.StringLiteral, Position.FirstCharacter));
            Add("\"sample \"literal\".get(1)", true, new Token<string>("sample ", TokenType.StringLiteral, Position.FirstCharacter));
            Add("\"sample ", false, Token.Unknown(Position.FirstCharacter));
            Add("\"sample \n\"", false, Token.Unknown(Position.FirstCharacter));
            Add("\"some literal, \\n\\t\"", true, new Token<string>("some literal, \n\t", TokenType.StringLiteral, Position.FirstCharacter));
            Add("\"\"", true, new Token<string>(string.Empty, TokenType.StringLiteral, Position.FirstCharacter));
            Add("123123123.()", false, Token.Unknown(Position.FirstCharacter));
        }
    }

    public class StringLiteralsFinishedCharacterData : TheoryData<string, char?>
    {
        public StringLiteralsFinishedCharacterData()
        {
            Add("\"sample literal\" next things", ' ');
            Add("\"sample \"literal\".get(1)", 'l');
            Add("\"sample ", null);
            Add("\"sample \n\"", '\n');
            Add("\"some literal, \\n\"", null);
            Add("\"\"", null);
            Add("123123123.()", '1');
        }
    }
}
