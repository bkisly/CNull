using CNull.Lexer.Tests.Data;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests.FactoriesTests
{
    public class NumericLiteralFactoryTests(LexerHelpersFixture fixture) : IClassFixture<LexerHelpersFixture>
    {
        [Theory, ClassData(typeof(IntegerLiteralsData))]
        public void CanBuildIntegerLiterals(string input, bool expectedResult, Token expectedToken)
            => TokenFactoriesTestsCore.TestTokensCreation(input, expectedResult, expectedToken, fixture);

        [Theory, ClassData(typeof(FloatingPointLiteralsData))]
        public void CanBuildFloatingPointLiterals(string input, bool expectedResult, Token expectedToken)
            => TokenFactoriesTestsCore.TestTokensCreation(input, expectedResult, expectedToken, fixture);

        [Theory, ClassData(typeof(NumericStateFinishingCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => TokenFactoriesTestsCore.TestFinishedCharacter(input, expectedCharacter, fixture);
    }
}
