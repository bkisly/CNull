using CNull.Lexer.Constants;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests.FactoriesTests
{
    internal class TokenFactoriesTestsCore
    {
        public static void TestTokensCreation(string input, bool expectedResult, Token expectedToken, LexerHelpersFixture fixture)
        {
            // Arrange

            fixture.Setup(input);

            // Act

            var lexer = new Lexer(fixture.CodeSourceMock.Object, fixture.ErrorHandlerMock.Object,
                fixture.CompilerConfigurationMock.Object);
            var token = lexer.GetNextToken();

            // Assert

            Assert.Equal(expectedResult, token.TokenType != TokenType.Unknown);
            Assert.Equivalent(expectedToken, token);
            Assert.Equivalent(expectedToken, lexer.LastToken);
        }

        public static void TestFinishedCharacter(string input, char? expectedCharacter, LexerHelpersFixture fixture)
        {
            // Arrange

            fixture.Setup(input);

            // Act

            var lexer = new Lexer(fixture.CodeSourceMock.Object, fixture.ErrorHandlerMock.Object,
                fixture.CompilerConfigurationMock.Object);
            lexer.GetNextToken();

            // Assert

            Assert.Equal(expectedCharacter, fixture.CodeSourceMock.Object.CurrentCharacter);
        }
    }
}
