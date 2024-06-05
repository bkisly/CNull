using CNull.Common.Configuration;
using CNull.Common.State;
using CNull.Lexer;
using CNull.Source;
using Microsoft.Extensions.Logging;
using System.Text;
using CNull.Semantics;

namespace CNull.IntegrationTests.AcceptanceTests.Fixtures
{
    public class InterpreterDependenciesFixture
    {
        public static Interpreter.Interpreter BuildInterpreter(string input)
        {
            var configuration = new InMemoryCNullConfiguration();
            var stateManager = new StateManager();
            var errorHandler = new ErrorHandler.ErrorHandler(new Mock<ILogger<ErrorHandler.ErrorHandler>>().Object,
                configuration, stateManager);

            var rawSource = new RawCodeSource(errorHandler, stateManager);
            var sourceProxy = new NewLineUnifierCodeSourceProxy(rawSource);

            var lexer = new Lexer.Lexer(sourceProxy, errorHandler, configuration);
            var commentsFilterProxy = new CommentsFilterLexerProxy(lexer);

            var parser = new Parser.Parser(commentsFilterProxy, errorHandler, stateManager, new Mock<ILogger<Parser.Parser>>().Object);
            var semantic = new SemanticAnalyzer(parser, errorHandler, stateManager);
            var interpreter = new Interpreter.Interpreter(semantic, errorHandler);

            stateManager.NotifyInputRequested(new Lazy<Stream>(() => new MemoryStream(Encoding.UTF8.GetBytes(input))));

            return interpreter;
        }
    }
}
