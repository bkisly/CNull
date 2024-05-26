using CNull.Common;
using CNull.Common.Configuration;
using CNull.Common.Mediators;
using CNull.Lexer;
using CNull.Parser;
using CNull.Parser.Errors;
using CNull.Parser.Productions;
using CNull.Source;
using Microsoft.Extensions.Logging;

namespace CNull.IntegrationTests
{
    public class ParserIntegrationTests
    {
        [Fact]
        public void CanParsePrograms()
        {
            // Arrange

            const string input = """
                                 import Module.Function1;
                                 import Module.Function2;

                                 // this is a sample comment

                                 int Main(bool parameter1, dict<int, string> parameter2)
                                 {
                                     int someVariable = 20;
                                     if(someVariable >= 10)
                                     {
                                         return someVariable;
                                     }
                                     else
                                     {
                                         someVariable = someVariable % 5;
                                         Foo();
                                     }
                                     
                                     return someVariable - 10;
                                 }

                                 void Foo()
                                 {
                                 }
                                 """;

            var configuration = new InMemoryCNullConfiguration();
            var mediator = new CoreComponentsMediator();
            var errorHandler = new ErrorHandler.ErrorHandler(new Mock<ILogger<ErrorHandler.ErrorHandler>>().Object,
                configuration, mediator);

            var rawSource = new RawCodeSource(errorHandler, mediator);
            var sourceProxy = new NewLineUnifierCodeSourceProxy(rawSource);

            var lexer = new Lexer.Lexer(sourceProxy, errorHandler, configuration);
            var commentsFilterProxy = new CommentsFilterLexerProxy(lexer);

            var parser = new Parser.Parser(commentsFilterProxy, errorHandler, mediator, new Mock<ILogger<Parser.Parser>>().Object);

            mediator.NotifyInputRequested(new Lazy<TextReader>(() => new StringReader(input)), "sample path");

            var expectedImports = new[]
            {
                new ImportDirective("Module", "Function1", Position.FirstCharacter),
                new ImportDirective("Module", "Function2", new Position(2, 1))
            };

            var expectedFunctions = new[]
            {
                new FunctionDefinition(
                    new PrimitiveType(PrimitiveTypes.Integer, new Position(6, 1)),
                    "Main",
                    [
                        new Parameter(
                            new PrimitiveType(PrimitiveTypes.Boolean, new Position(6, 10)),
                            "parameter1",
                            new Position(6, 10)),
                        new Parameter(
                            new DictionaryType(
                                new PrimitiveType(PrimitiveTypes.Integer, new Position(6, 32)),
                                new PrimitiveType(PrimitiveTypes.String, new Position(6, 37)),
                                new Position(6, 27)),
                            "parameter2",
                            new Position(6, 27))
                    ],
                    new BlockStatement(
                        [
                            new VariableDeclaration(
                                new PrimitiveType(PrimitiveTypes.Integer, new Position(8, 5)),
                                "someVariable",
                                new Position(8, 5),
                                new LiteralExpression<int>(20, new Position(8, 24))),
                            new IfStatement(
                                new GreaterThanOrEqualExpression(
                                    new IdentifierExpression("someVariable", new Position(9, 8)),
                                    new LiteralExpression<int>(10, new Position(9, 24)),
                                    new Position(9, 21)),
                                new BlockStatement(
                                    new[]
                                    {
                                        new ReturnStatement(
                                            new IdentifierExpression("someVariable", new Position(11, 16)),
                                            new Position(11, 9))
                                    },
                                    new Position(10, 5)),
                                null,
                                new BlockStatement(
                                    new[]
                                    {
                                        new ExpressionStatement(
                                            new IdentifierExpression("someVariable", new Position(15, 9)),
                                            new Position(15, 9),
                                            new ModuloExpression(
                                                new IdentifierExpression("someVariable", new Position(15, 24)),
                                                new LiteralExpression<int>(5, new Position(15, 39)),
                                                new Position(15, 37)))
                                    },
                                    new Position(14, 5)),
                                new Position(9, 5)),
                            new ReturnStatement(
                                new SubtractionExpression(
                                    new IdentifierExpression("someVariable", new Position(19, 12)),
                                    new LiteralExpression<int>(10, new Position(19, 27)),
                                    new Position(19, 25)),
                                new Position(19, 5))
                        ],
                        new Position(7, 1)),
                    new Position(6, 1)),
                new FunctionDefinition(
                    new ReturnType(new Position(22, 1)),
                    "Foo",
                    [],
                    new BlockStatement(
                        [],
                        new Position(23, 1)),
                    new Position(22, 1))
            };

            var expectedProgram = new Program("SamplePath", expectedImports, expectedFunctions);

            // Act

            var program = parser.Parse();

            // Assert

            Assert.NotNull(program);
            Assert.Equivalent(expectedProgram, program);
            Assert.Empty(errorHandler.Errors);
        }

        [Fact]
        public void CanProceedWithParsingWhenMissingSemicolon()
        {
            // Arrange

            const string input = """
                                 import Module.Function1;
                                 import Module.Function2;

                                 // this is a sample comment

                                 int Main(bool parameter1, dict<int, string> parameter2)
                                 {
                                     int someVariable = 20;
                                     if(someVariable >= 10)
                                     {
                                         return someVariable;
                                     }
                                     else
                                     {
                                         someVariable = someVariable % 5;
                                         Foo()
                                     }
                                     
                                     return someVariable - 10;
                                 }

                                 void Foo()
                                 {
                                 }
                                 """;

            var configuration = new InMemoryCNullConfiguration();
            var mediator = new CoreComponentsMediator();
            var errorHandler = new ErrorHandler.ErrorHandler(new Mock<ILogger<ErrorHandler.ErrorHandler>>().Object,
                configuration, mediator);

            var rawSource = new RawCodeSource(errorHandler, mediator);
            var sourceProxy = new NewLineUnifierCodeSourceProxy(rawSource);

            var lexer = new Lexer.Lexer(sourceProxy, errorHandler, configuration);
            var commentsFilterProxy = new CommentsFilterLexerProxy(lexer);

            var parser = new Parser.Parser(commentsFilterProxy, errorHandler, mediator, new Mock<ILogger<Parser.Parser>>().Object);

            mediator.NotifyInputRequested(new Lazy<TextReader>(() => new StringReader(input)), "sample path");

            var expectedImports = new[]
            {
                new ImportDirective("Module", "Function1", Position.FirstCharacter),
                new ImportDirective("Module", "Function2", new Position(2, 1))
            };

            var expectedFunctions = new[]
            {
                new FunctionDefinition(
                    new PrimitiveType(PrimitiveTypes.Integer, new Position(6, 1)),
                    "Main",
                    [
                        new Parameter(
                            new PrimitiveType(PrimitiveTypes.Boolean, new Position(6, 10)),
                            "parameter1",
                            new Position(6, 10)),
                        new Parameter(
                            new DictionaryType(
                                new PrimitiveType(PrimitiveTypes.Integer, new Position(6, 32)),
                                new PrimitiveType(PrimitiveTypes.String, new Position(6, 37)),
                                new Position(6, 27)),
                            "parameter2",
                            new Position(6, 27))
                    ],
                    new BlockStatement(
                        [
                            new VariableDeclaration(
                                new PrimitiveType(PrimitiveTypes.Integer, new Position(8, 5)),
                                "someVariable",
                                new Position(8, 5),
                                new LiteralExpression<int>(20, new Position(8, 24))),
                            new IfStatement(
                                new GreaterThanOrEqualExpression(
                                    new IdentifierExpression("someVariable", new Position(9, 8)),
                                    new LiteralExpression<int>(10, new Position(9, 24)),
                                    new Position(9, 21)),
                                new BlockStatement(
                                    new[]
                                    {
                                        new ReturnStatement(
                                            new IdentifierExpression("someVariable", new Position(11, 16)),
                                            new Position(11, 9))
                                    },
                                    new Position(10, 5)),
                                null,
                                new BlockStatement(
                                    new[]
                                    {
                                        new ExpressionStatement(
                                            new IdentifierExpression("someVariable", new Position(15, 9)),
                                            new Position(15, 9),
                                            new ModuloExpression(
                                                new IdentifierExpression("someVariable", new Position(15, 24)),
                                                new LiteralExpression<int>(5, new Position(15, 39)),
                                                new Position(15, 37)))
                                    },
                                    new Position(14, 5)),
                                new Position(9, 5)),
                            new ReturnStatement(
                                new SubtractionExpression(
                                    new IdentifierExpression("someVariable", new Position(19, 12)),
                                    new LiteralExpression<int>(10, new Position(19, 27)),
                                    new Position(19, 25)),
                                new Position(19, 5))
                        ],
                        new Position(7, 1)),
                    new Position(6, 1)),
                new FunctionDefinition(
                    new ReturnType(new Position(22, 1)),
                    "Foo",
                    [],
                    new BlockStatement(
                        [],
                        new Position(23, 1)),
                    new Position(22, 1))
            };

            var expectedProgram = new Program("SamplePath", expectedImports, expectedFunctions);

            // Act

            var program = parser.Parse();

            // Assert

            Assert.NotNull(program);
            Assert.Equivalent(expectedProgram, program);
            Assert.Single(errorHandler.Errors);
            Assert.Equivalent(new MissingKeywordOrOperatorError(";", new Position(17, 5)),
                errorHandler.Errors.FirstOrDefault());
        }
    }
}
