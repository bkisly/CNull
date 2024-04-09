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
            fixture.MockedBuffer = "// some comment";
            fixture.CodeSourceMock.Object.MoveToNext();

            var commentStateMock = new Mock<ILexerState>();
            Token discardedToken;
            commentStateMock.Setup(s => s.TryBuildToken(out discardedToken)).Returns(true);

            var state = new OperatorOrPunctorLexerState(fixture.ServicesContainerMock.Object, commentStateMock.Object);

            // Act

            var result = state.TryBuildToken(out var token);

            // Assert

            Assert.True(result);
            commentStateMock.Verify(s => s.TryBuildToken(out discardedToken), Times.Once);
        }
    }
}
