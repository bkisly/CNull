using CNull.Lexer.Tests.Data;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests.FactoriesTests
{
    public class CharLiteralFactoryTests(LexerHelpersFixture fixture) : IClassFixture<LexerHelpersFixture>
    {
        [Theory, ClassData(typeof(CharLiteralsData))]
        public void CanBuildCharLiterals(string input, bool expectedResult, Token expectedToken)
            => TokenFactoriesTestsCore.TestTokensCreation(input, expectedResult, expectedToken, fixture);

        [Theory, ClassData(typeof(CharLiteralsFinishCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => TokenFactoriesTestsCore.TestFinishedCharacter(input, expectedCharacter, fixture);
    }
}
