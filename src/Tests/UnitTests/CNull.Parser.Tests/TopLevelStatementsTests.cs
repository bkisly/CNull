using CNull.ErrorHandler.Errors;
using CNull.Lexer;
using CNull.Parser.Productions;
using CNull.Parser.Tests.Data;
using CNull.Parser.Tests.Fixtures;

namespace CNull.Parser.Tests
{
    public class TopLevelStatementsTests(ParserHelpersFixture fixture) : IClassFixture<ParserHelpersFixture>
    {
        [Theory, ClassData(typeof(TopLevelStatementsData))]
        public void CanBuildValidTopLevelStatements(IEnumerable<Token> tokens, Program expectedProgram)
        {
            // Arrange

            fixture.SetupTokensQueue(tokens);
            var parser = new Parser(fixture.Lexer.Object, fixture.ErrorHandler.Object, fixture.StateManager.Object, fixture.Logger.Object);

            // Act

            var program = parser.Parse();

            // Assert

            Assert.NotNull(program);
            Assert.Equivalent(expectedProgram, program);
            fixture.ErrorHandler.Verify(e => e.RaiseCompilationError(It.IsAny<ICompilationError>()), Times.Never);
            fixture.ErrorHandler.Verify(e => e.RaiseFatalCompilationError(It.IsAny<ICompilationError>()), Times.Never);
        }

        [Theory, ClassData(typeof(InvalidTopLevelStatementsData))]
        public void CannotBuildInvalidTopLevelStatements(IEnumerable<Token> tokens, ICompilationError expectedError)
        {
            // Arrange

            fixture.SetupTokensQueue(tokens);
            var parser = new Parser(fixture.Lexer.Object, fixture.ErrorHandler.Object, fixture.StateManager.Object, fixture.Logger.Object);

            // Act

            var program = parser.Parse();

            // Assert

            Assert.Null(program);
            fixture.ErrorHandler.Verify(e => e.RaiseCompilationError(expectedError), Times.Once);
            fixture.ErrorHandler.Verify(e => e.RaiseFatalCompilationError(expectedError), Times.Never);
        }
    }
}