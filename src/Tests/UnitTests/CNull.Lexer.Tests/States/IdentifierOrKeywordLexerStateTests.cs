using CNull.Lexer.States;
using CNull.Lexer.Tests.Data;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests.States
{
    public class IdentifierOrKeywordLexerStateTests(LexerStateFixture fixture) : IClassFixture<LexerStateFixture>
    {
        [Theory, ClassData(typeof(IdentifiersData))]
        public void CanBuildIdentifiers(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new IdentifierOrKeywordLexerState(fixture.ServicesContainerMock.Object), fixture);

        [Theory, ClassData(typeof(KeywordsData))]
        public void CanBuildKeywords(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new IdentifierOrKeywordLexerState(fixture.ServicesContainerMock.Object), fixture);

        [Theory, ClassData(typeof(IdentifiersLastCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => StateTestsCore.TestFinishedCharacter(input, expectedCharacter,
                new IdentifierOrKeywordLexerState(fixture.ServicesContainerMock.Object), fixture);
    }
}
