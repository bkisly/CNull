using CNull.Lexer.States;
using CNull.Lexer.Tests.Data;
using CNull.Source.Tests.Helpers;

namespace CNull.Lexer.Tests.States
{
    public class StringLexerStateTests(NewLineProxyFixture fixture) : IClassFixture<NewLineProxyFixture>
    {
        [Theory, ClassData(typeof(StringLiteralsData))]
        public void CanBuildStringLiterals(string input, bool expectedResult, Token expectedToken)
        {
            // Arrange

            fixture.Reset();
            fixture.MockedBuffer = input;
            fixture.CodeSourceMock.Object.MoveToNext();

            var state = new StringLiteralLexerState(fixture.CodeSourceMock.Object);

            // Act

            var result = state.TryBuildToken(out var token);

            // Assert

            Assert.Equal(expectedResult, result);
            Assert.Equivalent(expectedToken, token);
        }

        [Theory, ClassData(typeof(StringLiteralsFinishedCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
        {
            // Arrange

            fixture.Reset();
            fixture.MockedBuffer = input;
            fixture.CodeSourceMock.Object.MoveToNext();

            var state = new StringLiteralLexerState(fixture.CodeSourceMock.Object);

            // Act

            state.TryBuildToken(out _);

            // Assert

            Assert.Equal(expectedCharacter, fixture.CodeSourceMock.Object.CurrentCharacter);
        }
    }
}
