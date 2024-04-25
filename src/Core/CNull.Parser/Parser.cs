using CNull.Lexer;
using CNull.Lexer.Constants;
using CNull.Parser.Productions;

namespace CNull.Parser
{
    public class Parser : IParser
    {
        private readonly ILexer _lexer;
        private Token _currentToken = null!;

        public Parser(ILexer lexer)
        {
            _lexer = lexer;
            ConsumeToken();
        }

        public Program Parse()
        {
            while (_lexer.LastToken is not { TokenType: TokenType.End })
                _lexer.GetNextToken();

            return null;
        }

        private void ConsumeToken() => _currentToken = _lexer.GetNextToken();
    }
}
