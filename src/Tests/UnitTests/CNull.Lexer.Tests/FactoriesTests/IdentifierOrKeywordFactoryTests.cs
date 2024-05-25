using CNull.Lexer.Tests.Data;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests.FactoriesTests
{
    public class IdentifierOrKeywordFactoryTests(LexerHelpersFixture fixture) : IClassFixture<LexerHelpersFixture>
    {
        [Theory, ClassData(typeof(IdentifiersData))]
        public void CanBuildIdentifiers(string input, bool expectedResult, Token expectedToken)
            => TokenFactoriesTestsCore.TestTokensCreation(input, expectedResult, expectedToken, fixture);

        [Theory, ClassData(typeof(KeywordsData))]
        public void CanBuildKeywords(string input, bool expectedResult, Token expectedToken)
            => TokenFactoriesTestsCore.TestTokensCreation(input, expectedResult, expectedToken, fixture);

        [Theory, ClassData(typeof(IdentifiersLastCharacterData))]
        public void CanFinishIdentifiersAtProperCharacter(string input, char? expectedCharacter)
            => TokenFactoriesTestsCore.TestFinishedCharacter(input, expectedCharacter, fixture);

        [Theory, ClassData(typeof(KeywordsLastCharacterData))]
        public void CanFinishKeywordsAtProperCharacter(string input, char? expectedCharacter)
            => TokenFactoriesTestsCore.TestFinishedCharacter(input, expectedCharacter, fixture);
    }
}
