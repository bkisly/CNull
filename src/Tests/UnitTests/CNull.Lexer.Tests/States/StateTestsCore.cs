using CNull.Lexer.States;
using CNull.Lexer.Tests.Fixtures;
using CNull.Source.Tests.Helpers;

namespace CNull.Lexer.Tests.States
{
    internal class StateTestsCore
    {
        public static void TestTokensCreation(string input, bool expectedResult, Token expectedToken, ILexerState state, LexerStateFixture fixture)
        {
            // Arrange

            fixture.Reset();
            fixture.MockedBuffer = input;
            fixture.CodeSourceMock.Object.MoveToNext();

            // Act

            var result = state.TryBuildToken(out var token);

            // Assert

            Assert.Equal(expectedResult, result);
            Assert.Equivalent(expectedToken, token);
        }

        public static void TestFinishedCharacter(string input, char? expectedCharacter, ILexerState state, LexerStateFixture fixture)
        {
            // Arrange

            fixture.Reset();
            fixture.MockedBuffer = input;
            fixture.CodeSourceMock.Object.MoveToNext();

            // Act

            state.TryBuildToken(out _);

            // Assert

            Assert.Equal(expectedCharacter, fixture.CodeSourceMock.Object.CurrentCharacter);
        }
    }
}
