using System.Text;
using CNull.Common;
using CNull.Common.Configuration;
using CNull.Common.State;
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
                new(TokenType.ImportKeyword, Position.FirstCharacter),
                new Token<string>("CNull", TokenType.Identifier, new Position(1, 8)),
                new(TokenType.DotOperator, new Position(1, 13)),
                new Token<string>("Console", TokenType.Identifier, new Position(1, 14)),
                new(TokenType.SemicolonOperator, new Position(1, 21)),
                new(TokenType.IntKeyword, new Position(3, 1)),
                new Token<string>("Main", TokenType.Identifier, new Position(3, 5)),
                new(TokenType.LeftParenthesisOperator, new Position(3, 9)),
                new(TokenType.DictKeyword, new Position(3, 10)),
                new(TokenType.LessThanOperator, new Position(3, 14)),
                new(TokenType.IntKeyword, new Position(3, 15)),
                new(TokenType.CommaOperator, new Position(3, 18)),
                new(TokenType.StringKeyword, new Position(3, 20)),
                new(TokenType.GreaterThanOperator, new Position(3, 26)),
                new Token<string>("args", TokenType.Identifier, new Position(3, 28)),
                new(TokenType.RightParenthesisOperator, new Position(3, 32)),
                new(TokenType.OpenBlockOperator, new Position(4, 1)),
                new(TokenType.FloatKeyword, new Position(5, 3)),
                new Token<string>("a", TokenType.Identifier, new Position(5, 9)),
                new(TokenType.AssignmentOperator, new Position(5, 11)),
                new Token<float>(23.456f, TokenType.FloatLiteral, new Position(5, 13)),
                new(TokenType.SemicolonOperator, new Position(5, 19)),
                new Token<string>("a", TokenType.Identifier, new Position(6, 3)),
                new Token<string>("b", TokenType.Identifier, new Position(6, 5)),
                new(TokenType.CloseBlockOperator, new Position(7, 1)),
                new(TokenType.End, new Position(7, 1)),
                new(TokenType.End, new Position(7, 1)),
                new(TokenType.End, new Position(7, 1))
            };

            var configuration = new InMemoryCNullConfiguration();
            var stateManager = new StateManager();
            var errorHandler = new ErrorHandler.ErrorHandler(new Mock<ILogger<ErrorHandler.ErrorHandler>>().Object, 
                configuration, stateManager);

            var rawSource = new RawCodeSource(errorHandler, stateManager);
            var sourceProxy = new NewLineUnifierCodeSourceProxy(rawSource);

            var lexer = new Lexer.Lexer(sourceProxy, errorHandler, configuration);

            stateManager.NotifyInputRequested(new Lazy<Stream>(() =>
                new MemoryStream(Encoding.UTF8.GetBytes(testBuffer))));

            // Act

            var resultTokens = new List<Token>();
            for (var i = 0; i < expectedTokens.Count; i++)
                resultTokens.Add(lexer.GetNextToken());

            // Assert

            Assert.Equivalent(expectedTokens, resultTokens);
        }
    }
}
