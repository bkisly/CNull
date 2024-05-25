using CNull.Common;
using CNull.Lexer.Constants;

namespace CNull.Lexer.Tests.Data
{
    internal class CharLiteralsData : TheoryData<string, bool, Token>
    {
        public CharLiteralsData()
        {
            Add("'A' == something", true, new Token<char>('A', TokenType.CharLiteral, Position.FirstCharacter));
            Add("'\\'' == something", true, new Token<char>('\'', TokenType.CharLiteral, Position.FirstCharacter));
            Add("'\\n' == something", true, new Token<char>('\n', TokenType.CharLiteral, Position.FirstCharacter));
            Add("'\\t' == something", true, new Token<char>('\t', TokenType.CharLiteral, Position.FirstCharacter));
            Add(@"'\\' == something", true, new Token<char>('\\', TokenType.CharLiteral, Position.FirstCharacter));
            Add("'\\\"' == something", true, new Token<char>('"', TokenType.CharLiteral, Position.FirstCharacter));
            Add("'' == something", false, Token.Unknown(Position.FirstCharacter));
            Add("'AB' == something", false, Token.Unknown(Position.FirstCharacter));
            Add("'\n' == something", false, Token.Unknown(Position.FirstCharacter));
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
        }
    }
}
