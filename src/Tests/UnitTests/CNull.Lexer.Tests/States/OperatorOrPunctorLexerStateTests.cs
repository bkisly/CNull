using CNull.Common;
using CNull.Lexer.Constants;
using CNull.Lexer.States;
using CNull.Lexer.Tests.Data;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests.States
{
    public class OperatorOrPunctorLexerStateTests(LexerStateFixture fixture) : IClassFixture<LexerStateFixture>
    {
        [Theory, ClassData(typeof(OperatorsData))]
        public void CanBuildOperators(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new OperatorOrPunctorLexerState(fixture.ServicesContainerMock.Object,
                    new CommentLexerState(fixture.ServicesContainerMock.Object)), fixture);

        [Theory, ClassData(typeof(OperatorsLastCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => StateTestsCore.TestFinishedCharacter(input, expectedCharacter,
                new OperatorOrPunctorLexerState(fixture.ServicesContainerMock.Object,
                    new CommentLexerState(fixture.ServicesContainerMock.Object)), fixture);

        [Fact]
        public void CanSwitchToCommentState()
        {
            // Arrange

            fixture.Reset();
            fixture.Setup("// some comment");
            fixture.CodeSourceMock.Object.MoveToNext();

            var commentStateMock = new Mock<ILexerState>();
            commentStateMock.Setup(s => s.BuildToken()).Returns(new Token(TokenType.Comment, Position.FirstCharacter));

            var state = new OperatorOrPunctorLexerState(fixture.ServicesContainerMock.Object, commentStateMock.Object);

            // Act

            var token = state.BuildToken();

            // Assert

            Assert.Equal(TokenType.Comment, token.TokenType);
            commentStateMock.Verify(s => s.BuildToken(), Times.Once);
        }
    }
}
