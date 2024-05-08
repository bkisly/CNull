﻿using CNull.Common;
using CNull.Common.Configuration;
using CNull.Common.Mediators;
using CNull.ErrorHandler.Errors;
using CNull.Lexer;
using CNull.Lexer.Factories;
using CNull.Lexer.ServicesContainers;
using CNull.Parser.Enums;
using CNull.Parser.Productions;
using CNull.Parser.Tests.Fixtures;
using CNull.Source.Repositories;
using CNull.Source;

namespace CNull.IntegrationTests.CrossComponents
{
    public class ParserCrossTests(ParserHelpersFixture fixture) : IClassFixture<ParserHelpersFixture>
    {
        [Fact]
        public void CanParsePrograms()
        {
            // Arrange

            var input = """
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

            var reader = new StringReader(input);
            var repository = new InputRepository();
            repository.SetupStream(reader);

            var rawSource = new RawCodeSource(repository, fixture.ErrorHandler.Object,
                new Mock<ICoreComponentsMediator>().Object);

            var sourceProxy = new NewLineUnifierProxy(rawSource);

            var lexerServicesContainer = new LexerStateServicesContainer(sourceProxy, fixture.ErrorHandler.Object,
                new InternalCompilerConfiguration());

            var stateFactory = new LexerStateFactory(lexerServicesContainer);

            var lexer = new Lexer.Lexer(lexerServicesContainer, stateFactory);
            var commentsFilterProxy = new CommentsFilterLexerProxy(lexer);

            var parser = new Parser.Parser(commentsFilterProxy, fixture.ErrorHandler.Object, fixture.Logger.Object);

            sourceProxy.MoveToNext();

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
                    new[]
                    {
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
                    },
                    new BlockStatement(
                        new IBasicStatement[]
                        {
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
                        },
                        new Position(7, 1)),
                    new Position(6, 1)),
                new FunctionDefinition(
                    new ReturnType(new Position(22, 1)),
                    "Foo",
                    new List<Parameter>(),
                    new BlockStatement(
                        new List<IBasicStatement>(),
                        new Position(23, 1)),
                    new Position(22, 1))
            };

            var expectedProgram = new Program(expectedImports, expectedFunctions);

            // Act

            var program = parser.Parse();

            // Assert

            Assert.NotNull(program);
            Assert.Equivalent(expectedProgram, program);
            fixture.ErrorHandler.Verify(e => e.RaiseCompilationError(It.IsAny<ICompilationError>()), Times.Never);
            fixture.ErrorHandler.Verify(e => e.RaiseSourceError(It.IsAny<ISourceError>()), Times.Never);
        }
    }
}