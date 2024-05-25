using CNull.Common;
using CNull.Common.Configuration;
using CNull.Common.Mediators;
using CNull.Lexer;
using CNull.Lexer.Constants;
using CNull.Source;
using Microsoft.Extensions.Logging;

namespace CNull.IntegrationTests
{
    public class LexerIntegrationTests
    {
        [Fact]
        public void CanIntegrateWithAllComponents()
        {
            // Arrange

            const string testBuffer = """
                                      import CNull.Console;

                                      int Main(dict<int, string> args)
                                      {
                                        float a = 23.456;
                                        a
                                      """ + "\r" + "b\r\n}";

            var expectedTokens = new List<Token>
            {
                new Token(TokenType.ImportKeyword, Position.FirstCharacter),
                new Token<string>("CNull", TokenType.Identifier, new Position(1, 8)),
                new Token(TokenType.DotOperator, new Position(1, 13)),
                new Token<string>("Console", TokenType.Identifier, new Position(1, 14)),
                new Token(TokenType.SemicolonOperator, new Position(1, 21)),
                new Token(TokenType.IntKeyword, new Position(3, 1)),
                new Token<string>("Main", TokenType.Identifier, new Position(3, 5)),
                new Token(TokenType.LeftParenthesisOperator, new Position(3, 9)),
                new Token(TokenType.DictKeyword, new Position(3, 10)),
                new Token(TokenType.LessThanOperator, new Position(3, 14)),
                new Token(TokenType.IntKeyword, new Position(3, 15)),
                new Token(TokenType.CommaOperator, new Position(3, 18)),
                new Token(TokenType.StringKeyword, new Position(3, 20)),
                new Token(TokenType.GreaterThanOperator, new Position(3, 26)),
                new Token<string>("args", TokenType.Identifier, new Position(3, 28)),
                new Token(TokenType.RightParenthesisOperator, new Position(3, 32)),
                new Token(TokenType.OpenBlockOperator, new Position(4, 1)),
                new Token(TokenType.FloatKeyword, new Position(5, 3)),
                new Token<string>("a", TokenType.Identifier, new Position(5, 9)),
                new Token(TokenType.AssignmentOperator, new Position(5, 11)),
                new Token<float>(23.456f, TokenType.FloatLiteral, new Position(5, 13)),
                new Token(TokenType.SemicolonOperator, new Position(5, 19)),
                new Token<string>("a", TokenType.Identifier, new Position(6, 3)),
                new Token<string>("b", TokenType.Identifier, new Position(6, 5)),
                new Token(TokenType.CloseBlockOperator, new Position(7, 1)),
                new Token(TokenType.End, new Position(7, 1)),
                new Token(TokenType.End, new Position(7, 1)),
                new Token(TokenType.End, new Position(7, 1)),
            };

            var configuration = new InMemoryCNullConfiguration();
            var mediator = new CoreComponentsMediator();
            var errorHandler = new ErrorHandler.ErrorHandler(new Mock<ILogger<ErrorHandler.ErrorHandler>>().Object, configuration);

            var rawSource = new RawCodeSource(errorHandler, mediator);
            var sourceProxy = new NewLineUnifierCodeSourceProxy(rawSource);

            var lexer = new Lexer.Lexer(sourceProxy, errorHandler, configuration);

            mediator.NotifyInputRequested(new Lazy<TextReader>(() => new StringReader(testBuffer)), "sample path");

            // Act

            var resultTokens = new List<Token>();
            for (var i = 0; i < expectedTokens.Count; i++)
                resultTokens.Add(lexer.GetNextToken());

            // Assert

            foreach (var (expectedToken, actualToken) in expectedTokens.Zip(resultTokens))
            {
                Assert.Equivalent(expectedToken, actualToken);
            }
        }
    }
}
