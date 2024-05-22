using CNull.Common;
using CNull.Lexer.Constants;
using CNull.Lexer.Tests.Fixtures;

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
                new Token(TokenType.ImportKeyword, Position.FirstCharacter),
                new Token<string>("CNull", TokenType.Identifier, Position.FirstCharacter),
                new Token(TokenType.DotOperator, Position.FirstCharacter),
                new Token<string>("Console", TokenType.Identifier, Position.FirstCharacter),
                new Token(TokenType.SemicolonOperator, Position.FirstCharacter),
                new Token(TokenType.IntKeyword, Position.FirstCharacter),
                new Token<string>("Main", TokenType.Identifier, Position.FirstCharacter),
                new Token(TokenType.LeftParenthesisOperator, Position.FirstCharacter),
                new Token(TokenType.DictKeyword, Position.FirstCharacter),
                new Token(TokenType.LessThanOperator, Position.FirstCharacter),
                new Token(TokenType.IntKeyword, Position.FirstCharacter),
                new Token(TokenType.CommaOperator, Position.FirstCharacter),
                new Token(TokenType.StringKeyword, Position.FirstCharacter),
                new Token(TokenType.GreaterThanOperator, Position.FirstCharacter),
                new Token<string>("args", TokenType.Identifier, Position.FirstCharacter),
                new Token(TokenType.RightParenthesisOperator, Position.FirstCharacter),
                new Token(TokenType.OpenBlockOperator, Position.FirstCharacter),
                new Token(TokenType.FloatKeyword, Position.FirstCharacter),
                new Token<string>("a", TokenType.Identifier, Position.FirstCharacter),
                new Token(TokenType.AssignmentOperator, Position.FirstCharacter),
                new Token<float>(23.456f, TokenType.FloatLiteral, Position.FirstCharacter),
                new Token(TokenType.SemicolonOperator, Position.FirstCharacter),
                new Token(TokenType.CloseBlockOperator, Position.FirstCharacter),
                new Token(TokenType.End, Position.FirstCharacter),
                new Token(TokenType.End, Position.FirstCharacter),
                new Token(TokenType.End, Position.FirstCharacter),
            };

            fixture.Setup(testBuffer);

            var lexer = new Lexer(fixture.CodeSourceMock.Object, fixture.ErrorHandlerMock.Object,
                fixture.CompilerConfigurationMock.Object);

            // Act

            var resultTokens = new List<Token>();
            for (var i = 0; i < expectedTokens.Count; i++)
                resultTokens.Add(lexer.GetNextToken());

            // Assert

            Assert.Equivalent(expectedTokens, resultTokens);
        }
    }
}
