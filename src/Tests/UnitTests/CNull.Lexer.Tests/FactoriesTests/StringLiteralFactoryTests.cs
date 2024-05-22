using CNull.Lexer.Tests.Data;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests.FactoriesTests
{
    public class StringLiteralFactoryTests(LexerHelpersFixture fixture) : IClassFixture<LexerHelpersFixture>
    {
        [Theory, ClassData(typeof(StringLiteralsData))]
        public void CanBuildStringLiterals(string input, bool expectedResult, Token expectedToken)
            => TokenFactoriesTestsCore.TestTokensCreation(input, expectedResult, expectedToken, fixture);

        [Theory, ClassData(typeof(StringLiteralsFinishedCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => TokenFactoriesTestsCore.TestFinishedCharacter(input, expectedCharacter, fixture);
    }
}
