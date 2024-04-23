using CNull.Lexer;
using CNull.Lexer.Constants;

namespace CNull.Parser
{
    public class Parser(ILexer lexer) : IParser
    {
        public object Parse()
        {
            while (lexer.LastToken is not { TokenType: TokenType.End })
                lexer.GetNextToken();

            return null;
        }
    }
}
