using CNull.Common;
using CNull.Common.State;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors;
using CNull.ErrorHandler.Exceptions;
using CNull.Lexer;
using CNull.Lexer.Constants;
using Microsoft.Extensions.Logging;

namespace CNull.Parser.Tests.Fixtures
{
    public class ParserHelpersFixture
    {
        private Queue<Token> _tokens = new();

        public Mock<ILexer> Lexer { get; private set; } = null!;
        public Mock<IErrorHandler> ErrorHandler { get; private set; } = null!;
        public Mock<ILogger<IParser>> Logger { get; private set; } = null!;
        public Mock<IStateManager> StateManager { get; private set; } = null!;

        public ParserHelpersFixture()
        {
            ResetFixture();
        }

        public void SetupTokensQueue(IEnumerable<Token> tokens)
        {
            ResetFixture();
            _tokens = new Queue<Token>(tokens);
        }

        private Token DequeueToken()
        {
            return _tokens.Count != 0 ? _tokens.Dequeue() : new Token(TokenType.End, Position.FirstCharacter);
        }

        public void ResetFixture()
        {
            Lexer = new Mock<ILexer>();
            Lexer.Setup(l => l.GetNextToken()).Returns(DequeueToken);

            ErrorHandler = new Mock<IErrorHandler>();
            ErrorHandler.Setup(e => e.RaiseCompilationError(It.IsAny<ICompilationError>()));
            ErrorHandler.Setup(e => e.RaiseFatalCompilationError(It.IsAny<ICompilationError>())).Returns(new FatalErrorException(""));

            StateManager = new Mock<IStateManager>();
            StateManager.Setup(m => m.CurrentSourcePath).Returns("<unknown>");
            StateManager.Setup(m => m.CurrentModuleName).Returns("Program");

            Logger = new Mock<ILogger<IParser>>();
        }
    }
}
