using CNull.Lexer.Constants;

namespace CNull.Lexer.Tests.Data
{
    internal class CommentsData : TheoryData<string, bool, Token>
    {
        public CommentsData()
        {
            Add(" some nice comment", true, new Token<string>("some nice comment", TokenType.Comment));
            Add("some nice comment", true, new Token<string>("some nice comment", TokenType.Comment));
            Add("\t\tsome\nnice comment", true, new Token<string>("some", TokenType.Comment));
            Add("\nsomenice comment", true, new Token<string>("", TokenType.Comment));
            Add("///////////////", true, new Token<string>("///////////////", TokenType.Comment));
        }
    }

    internal class CommentsNextCharacterData : TheoryData<string, char?>
    {
        public CommentsNextCharacterData()
        {
            Add(" some nice comment", null);
            Add("some\n\nnice comment", '\n');
            Add("\t\tsome\nnice comment", 'n');
            Add("\nsomenice comment", 's');
            Add("/abcde", null);
            Add("abcdeabcdeabcde", null);
            Add("/////////////////", null);
        }
    }
}
