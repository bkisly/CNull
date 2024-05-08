using CNull.Common;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors;
using CNull.Lexer;
using CNull.Lexer.Constants;
using Microsoft.Extensions.Logging;

namespace CNull.Parser.Tests.Fixtures
{
    public class ParserHelpersFixture
    {
        private Queue<Token> _tokens = new();

        public Mock<ILexer> Lexer { get; }
        public Mock<IErrorHandler> ErrorHandler { get; }
        public Mock<ILogger<IParser>> Logger { get; }

        public ParserHelpersFixture()
        {
            Lexer = new Mock<ILexer>();
            Lexer.Setup(l => l.GetNextToken()).Returns(DequeueToken);

            ErrorHandler = new Mock<IErrorHandler>();
            ErrorHandler.Setup(e => e.RaiseCompilationError(It.IsAny<ICompilationError>()));

            Logger = new Mock<ILogger<IParser>>();
        }

        public void SetupTokensQueue(IEnumerable<Token> tokens)
        {
            _tokens = new Queue<Token>(tokens);
        }

        private Token DequeueToken()
        {
            return _tokens.Count != 0 ? _tokens.Dequeue() : new Token(TokenType.End, Position.FirstCharacter);
        }
    }
}
