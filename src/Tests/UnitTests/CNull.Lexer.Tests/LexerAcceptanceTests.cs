using CNull.Common;
using CNull.Lexer.Constants;
using CNull.Lexer.Tests.Fixtures;
using FluentAssertions;

namespace CNull.Lexer.Tests
{
    public class LexerAcceptanceTests(LexerHelpersFixture fixture) : IClassFixture<LexerHelpersFixture>
    {
        [Fact]
        public void CanBuildTokens()
        {
            // Arrange

            const string testBuffer = """
                                      import CNull.Console;

                                      int Main(dict<int, string> args)
                                      {
                                        float a = 23.456;
                                      }
                                      """;

            var expectedTokens = new List<Token>
            {
                new(TokenType.ImportKeyword, Position.FirstCharacter),
                new Token<string>("CNull", TokenType.Identifier, new Position(1, 8)),
                new(TokenType.DotOperator, new Position(1, 13)),
                new Token<string>("Console", TokenType.Identifier, new Position(1, 14)),
                new(TokenType.SemicolonOperator, new Position(1, 21)),
                new(TokenType.IntKeyword, new Position(1, 26)),
                new Token<string>("Main", TokenType.Identifier, new Position(1, 30)),
                new(TokenType.LeftParenthesisOperator, new Position(1, 34)),
                new(TokenType.DictKeyword, new Position(1, 35)),
                new(TokenType.LessThanOperator, new Position(1, 39)),
                new(TokenType.IntKeyword, new Position(1, 40)),
                new(TokenType.CommaOperator, new Position(1, 43)),
                new(TokenType.StringKeyword, new Position(1, 45)),
                new(TokenType.GreaterThanOperator, new Position(1, 51)),
                new Token<string>("args", TokenType.Identifier, new Position(1, 53)),
                new(TokenType.RightParenthesisOperator, new Position(1, 57)),
                new(TokenType.OpenBlockOperator, new Position(1, 60)),
                new(TokenType.FloatKeyword, new Position(1, 65)),
                new Token<string>("a", TokenType.Identifier, new Position(1, 71)),
                new(TokenType.AssignmentOperator, new Position(1, 73)),
                new Token<float>(23.456f, TokenType.FloatLiteral, new Position(1, 75)),
                new(TokenType.SemicolonOperator, new Position(1, 81)),
                new(TokenType.CloseBlockOperator, new Position(1, 84)),
                new(TokenType.End, new Position(1, 86)),
                new(TokenType.End, new Position(1, 86)),
                new(TokenType.End, new Position(1, 86))
            };

            fixture.Setup(testBuffer);

            var lexer = new Lexer(fixture.CodeSourceMock.Object, fixture.ErrorHandlerMock.Object,
                fixture.CompilerConfigurationMock.Object);

            // Act

            var resultTokens = new List<Token>();
            for (var i = 0; i < expectedTokens.Count; i++)
                resultTokens.Add(lexer.GetNextToken());

            // Assert

            resultTokens.Should().BeEquivalentTo(expectedTokens);
        }
    }
}
