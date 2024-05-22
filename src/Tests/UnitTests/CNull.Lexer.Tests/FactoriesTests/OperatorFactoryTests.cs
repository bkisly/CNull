using CNull.Lexer.Tests.Data;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests.FactoriesTests
{
    public class OperatorFactoryTests(LexerHelpersFixture fixture) : IClassFixture<LexerHelpersFixture>
    {
        [Theory, ClassData(typeof(OperatorsData))]
        public void CanBuildOperators(string input, bool expectedResult, Token expectedToken)
            => TokenFactoriesTestsCore.TestTokensCreation(input, expectedResult, expectedToken, fixture);

        [Theory, ClassData(typeof(OperatorsLastCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => TokenFactoriesTestsCore.TestFinishedCharacter(input, expectedCharacter, fixture);
    }
}
