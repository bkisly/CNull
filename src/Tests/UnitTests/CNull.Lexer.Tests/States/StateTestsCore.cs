using CNull.Lexer.Constants;
using CNull.Lexer.States;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests.States
{
    internal class StateTestsCore
    {
        public static void TestTokensCreation(string input, bool expectedResult, Token expectedToken, ILexerState state, LexerStateFixture fixture)
        {
            // Arrange

            fixture.Setup(input);
            fixture.CodeSourceMock.Object.MoveToNext();

            // Act

            var token = state.BuildToken();

            // Assert

            Assert.Equal(expectedResult, token.TokenType != TokenType.Unknown);
            Assert.Equivalent(expectedToken, token);
        }

        public static void TestFinishedCharacter(string input, char? expectedCharacter, ILexerState state, LexerStateFixture fixture)
        {
            // Arrange

            fixture.Setup(input);
            fixture.CodeSourceMock.Object.MoveToNext();

            // Act

            state.BuildToken();

            // Assert

            Assert.Equal(expectedCharacter, fixture.CodeSourceMock.Object.CurrentCharacter);
        }
    }
}
