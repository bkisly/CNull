using CNull.Lexer.States;
using CNull.Lexer.Tests.Helpers;
using CNull.Source.Tests.Helpers;

namespace CNull.Lexer.Tests.States
{
    public class NumericLexerStateTests(NewLineProxyFixture fixture) : IClassFixture<NewLineProxyFixture>
    {
        [Theory, ClassData(typeof(IntegerLiteralsData))]
        public void CanBuildIntegerLiterals(string input, bool expectedValid, Token expectedToken)
            => CanBuildNumericLiterals(input, expectedValid, expectedToken);

        [Theory, ClassData(typeof(FloatingPointLiteralsData))]
        public void CanBuildFloatingPointLiterals(string input, bool expectedValid, Token expectedToken)
            => CanBuildNumericLiterals(input, expectedValid, expectedToken);

        [Theory, ClassData(typeof(NumericStateFinishingCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCurrentCharacter)
        {
            // Arrange

            fixture.Reset();
            fixture.MockedBuffer = input;
            fixture.CodeSourceMock.Object.MoveToNext();

            var state = new NumericLexerState(fixture.CodeSourceMock.Object);

            // Act

            state.TryBuildToken(out _);

            // Assert

            Assert.Equal(expectedCurrentCharacter, fixture.CodeSourceMock.Object.CurrentCharacter);
        }

        private void CanBuildNumericLiterals(string input, bool expectedValid, Token expectedToken)
        {
            // Arrange

            fixture.Reset();
            fixture.MockedBuffer = input;
            fixture.CodeSourceMock.Object.MoveToNext();

            var state = new NumericLexerState(fixture.CodeSourceMock.Object);

            // Act

            var result = state.TryBuildToken(out var token);

            // Assert

            Assert.Equal(expectedValid, result);
            Assert.Equivalent(expectedToken, token);
        }
    }
}
