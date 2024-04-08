using CNull.Lexer.Constants;

namespace CNull.Lexer.Tests.Data
{
    public class StringLiteralsData : TheoryData<string, bool, Token>
    {
        public StringLiteralsData()
        {
            Add("\"sample literal\" next things", true, new Token<string>("sample literal", TokenType.StringLiteral));
            Add("\"sample \"literal\".get(1)", true, new Token<string>("sample ", TokenType.StringLiteral));
            Add("\"sample ", false, Token.Unknown());
            Add("\"sample \n\"", false, Token.Unknown());
            Add("\"some literal, \\n\\t\"", true, new Token<string>("some literal, \n\t", TokenType.StringLiteral));
            Add("\"\"", true, new Token<string>(string.Empty, TokenType.StringLiteral));
            Add("123123123.()", false, Token.Unknown());
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
