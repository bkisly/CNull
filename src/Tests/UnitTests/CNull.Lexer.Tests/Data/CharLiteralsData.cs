using CNull.Lexer.Constants;

namespace CNull.Lexer.Tests.Data
{
    internal class CharLiteralsData : TheoryData<string, bool, Token>
    {
        public CharLiteralsData()
        {
            Add("'A' == something", true, new Token<char>('A', TokenType.CharLiteral));
            Add("'\\'' == something", true, new Token<char>('\'', TokenType.CharLiteral));
            Add("'\\n' == something", true, new Token<char>('\n', TokenType.CharLiteral));
            Add("'\\t' == something", true, new Token<char>('\t', TokenType.CharLiteral));
            Add(@"'\\' == something", true, new Token<char>('\\', TokenType.CharLiteral));
            Add("'\\\"' == something", true, new Token<char>('"', TokenType.CharLiteral));
            Add("'' == something", false, Token.Unknown());
            Add("'AB' == something", false, Token.Unknown());
            Add("'\n' == something", false, Token.Unknown());
            Add("ABCDE == something", false, Token.Unknown());
        }
    }

    internal class CharLiteralsFinishCharacterData : TheoryData<string, char?>
    {
        public CharLiteralsFinishCharacterData()
        {
            Add("'A' == something", ' ');
            Add("'\\''== something", '=');
            Add("'\\n'", null);
            Add("'\\t''something", '\'');
            Add(@"'\\' == something", ' ');
            Add("ABCDE == something", 'A');
        }
    }
}
