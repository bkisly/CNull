using CNull.Lexer.States;
using CNull.Lexer.Tests.Data;
using CNull.Source.Tests.Helpers;

namespace CNull.Lexer.Tests.States
{
    public class OperatorOrPunctorLexerStateTests(NewLineProxyFixture fixture) : IClassFixture<NewLineProxyFixture>
    {
        [Theory, ClassData(typeof(OperatorsData))]
        public void CanBuildOperators(string input, bool expectedResult, Token expectedToken)
        {
            // Arrange

            fixture.Reset();
            fixture.MockedBuffer = input;
            fixture.CodeSourceMock.Object.MoveToNext();

            var commentStateMock = new Mock<ILexerState>();
            Token discardedToken;
            commentStateMock.Setup(s => s.TryBuildToken(out discardedToken)).Returns(true);

            var state = new OperatorOrPunctorLexerState(fixture.CodeSourceMock.Object, commentStateMock.Object);

            // Act

            var result = state.TryBuildToken(out var token);

            // Assert

            Assert.Equal(expectedResult, result);
            Assert.Equivalent(expectedToken, token);
        }

        [Theory, ClassData(typeof(OperatorsLastCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
        {
            // Arrange

            fixture.Reset();
            fixture.MockedBuffer = input;
            fixture.CodeSourceMock.Object.MoveToNext();

            var commentStateMock = new Mock<ILexerState>();
            Token discardedToken;
            commentStateMock.Setup(s => s.TryBuildToken(out discardedToken)).Returns(true);

            var state = new OperatorOrPunctorLexerState(fixture.CodeSourceMock.Object, commentStateMock.Object);

            // Act

            state.TryBuildToken(out _);

            // Assert

            Assert.Equal(expectedCharacter, fixture.CodeSourceMock.Object.CurrentCharacter);
        }

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

            var state = new OperatorOrPunctorLexerState(fixture.CodeSourceMock.Object, commentStateMock.Object);

            // Act

            var result = state.TryBuildToken(out var token);

            // Assert

            Assert.True(result);
            commentStateMock.Verify(s => s.TryBuildToken(out discardedToken), Times.Once);
        }
    }
}
