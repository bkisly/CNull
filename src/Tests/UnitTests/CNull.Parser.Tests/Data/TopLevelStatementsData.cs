using CNull.Common;
using CNull.Lexer;
using CNull.Lexer.Constants;
using CNull.Parser.Enums;
using CNull.Parser.Productions;

namespace CNull.Parser.Tests.Data
{
    public class TopLevelStatementsData : TheoryData<IEnumerable<Token>, Program>
    {
        public TopLevelStatementsData()
        {
            AddStandardCaseData();
        }

        private void AddStandardCaseData()
        {
            var tokens = new[]
            {
                new Token(TokenType.ImportKeyword, Position.FirstCharacter),
                new Token<string>("Module", TokenType.Identifier, new Position(1, 2)),
                new Token(TokenType.DotOperator, new Position(1, 3)),
                new Token<string>("Function", TokenType.Identifier, new Position(1, 4)),
                new Token(TokenType.SemicolonOperator, new Position(1, 5)),

                new Token(TokenType.ImportKeyword, new Position(2, 1)),
                new Token<string>("AnotherModule", TokenType.Identifier, new Position(2, 2)),
                new Token(TokenType.DotOperator, new Position(2, 3)),
                new Token<string>("AnotherFunction", TokenType.Identifier, new Position(2, 4)),
                new Token(TokenType.SemicolonOperator, new Position(2, 5)),

                new Token(TokenType.VoidKeyword, new Position(3, 1)),
                new Token<string>("Main", TokenType.Identifier, new Position(3, 2)),
                new Token(TokenType.LeftParenthesisOperator, new Position(3, 3)),
                new Token(TokenType.RightParenthesisOperator, new Position(3, 4)),
                new Token(TokenType.OpenBlockOperator, new Position(4, 1)),
                new Token(TokenType.CloseBlockOperator, new Position(5, 1)),

                new Token(TokenType.IntKeyword, new Position(6, 1)),
                new Token<string>("Foo", TokenType.Identifier, new Position(6, 2)),
                new Token(TokenType.LeftParenthesisOperator, new Position(6, 3)),
                new Token(TokenType.BoolKeyword, new Position(6, 4)),
                new Token<string>("parameter", TokenType.Identifier, new Position(6, 5)),
                new Token(TokenType.CommaOperator, new Position(6, 6)),
                new Token(TokenType.FloatKeyword, new Position(6, 7)),
                new Token<string>("anotherParameter", TokenType.Identifier, new Position(6, 8)),
                new Token(TokenType.RightParenthesisOperator, new Position(6, 9)),
                new Token(TokenType.OpenBlockOperator, new Position(7, 1)),
                new Token(TokenType.CloseBlockOperator, new Position(8, 1)),
            };

            var expectedImports = new[]
            {
                new ImportDirective("Module", "Function", Position.FirstCharacter),
                new ImportDirective("AnotherModule", "AnotherFunction", new Position(2, 1))
            };

            var expectedFunctions = new[]
            {
                new FunctionDefinition(
                    new ReturnType(new Position(3, 1)), 
                    "Main", 
                    new List<Parameter>(),
                    new BlockStatement(
                        new List<IBasicStatement>(), 
                        new Position(4, 1)), 
                    new Position(3, 1)),

                new FunctionDefinition(
                    new PrimitiveType(PrimitiveTypes.Integer, new Position(6, 1)),
                    "Foo",
                    new []
                    {
                        new Parameter(new PrimitiveType(PrimitiveTypes.Boolean, new Position(6, 4)), "parameter", new Position(6, 4)),
                        new Parameter(new PrimitiveType(PrimitiveTypes.Float, new Position(6, 7)), "anotherParameter", new Position(6, 7)),
                    },
                    new BlockStatement(
                        new List<IBasicStatement>(),
                        new Position(7, 1)),
                    new Position(6, 1)),
            };

            var expectedProgram = new Program(expectedImports, expectedFunctions);
            Add(tokens, expectedProgram);
        }
    }
}
