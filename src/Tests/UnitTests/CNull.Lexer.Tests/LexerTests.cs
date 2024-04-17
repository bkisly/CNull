using CNull.Common;
using CNull.Lexer.Constants;
using CNull.Lexer.Factories;
using CNull.Lexer.States;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests
{
    public class LexerTests(LexerStateFixture fixture) : IClassFixture<LexerStateFixture>
    {
        [Fact]
        public void CanBuildTokens()
        {
            // Arrange

            var token1 = new Token(TokenType.BoolKeyword, Position.FirstCharacter);
            var token2 = new Token(TokenType.Comment, Position.FirstCharacter);
            var token3 = Token.Unknown(Position.FirstCharacter);
            var endToken = new Token(TokenType.End, Position.FirstCharacter);
                                
            var stateMock1 = new Mock<ILexerState>();
            stateMock1.Setup(s => s.BuildToken())
                .Returns(token1)
                .Callback(fixture.CodeSourceMock.Object.MoveToNext);

            var stateMock2 = new Mock<ILexerState>();
            stateMock2.Setup(s => s.BuildToken())
                .Returns(token2)
                .Callback(fixture.CodeSourceMock.Object.MoveToNext);

            var stateMock3 = new Mock<ILexerState>();
            stateMock3.Setup(s => s.BuildToken())
                .Returns(token3)
                .Callback(fixture.CodeSourceMock.Object.MoveToNext);

            var factoryMock = new Mock<ILexerStateFactory>();
            factoryMock.Setup(f => f.Create('A')).Returns(stateMock1.Object);
            factoryMock.Setup(f => f.Create('B')).Returns(stateMock2.Object);
            factoryMock.Setup(f => f.Create('C')).Returns(stateMock3.Object);

            fixture.Reset();
            fixture.MockedBuffer = "  \n\tA B BBA C B\nB\t\nA";
            fixture.CodeSourceMock.Object.MoveToNext();

            var lexer = new Lexer(fixture.ServicesContainerMock.Object, factoryMock.Object);
            var expectedResults = new List<Token>
            {
                token1, token2, token2, token2, token1, token3, token2, token2, token1,
                endToken, endToken, endToken, endToken,
            };

            // Act

            var result = new List<Token>();
            for(var i = 0; i < 13; i++)
                result.Add(lexer.GetNextToken());

            // Assert

            Assert.Equivalent(expectedResults, result);
            Assert.Equivalent(endToken, lexer.LastToken);
        }
    }
}
