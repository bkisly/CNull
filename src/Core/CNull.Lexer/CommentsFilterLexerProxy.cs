using CNull.Lexer.Constants;

namespace CNull.Lexer
{
    /// <summary>
    /// Proxy responsible for filtering out comment tokens.
    /// </summary>
    /// <param name="lexer"></param>
    public class CommentsFilterLexerProxy(ILexer lexer) : ILexer
    {
        public Token? LastToken => lexer.LastToken;

        public Token GetNextToken()
        {
            var token = lexer.GetNextToken();

            while (token.TokenType == TokenType.Comment)
                token = lexer.GetNextToken();

            return token;
        }
    }
}
