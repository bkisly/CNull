using CNull.Lexer.States;
using CNull.Lexer.Tests.Data;
using CNull.Source.Tests.Helpers;

namespace CNull.Lexer.Tests.States
{
    public class IdentifierOrKeywordLexerStateTests(NewLineProxyFixture fixture) : IClassFixture<NewLineProxyFixture>
    {
        [Theory, ClassData(typeof(IdentifiersData))]
        public void CanBuildIdentifiers(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new IdentifierOrKeywordLexerState(fixture.CodeSourceMock.Object), fixture);

        [Theory, ClassData(typeof(KeywordsData))]
        public void CanBuildKeywords(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new IdentifierOrKeywordLexerState(fixture.CodeSourceMock.Object), fixture);

        [Theory, ClassData(typeof(IdentifiersLastCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => StateTestsCore.TestFinishedCharacter(input, expectedCharacter,
                new IdentifierOrKeywordLexerState(fixture.CodeSourceMock.Object), fixture);
    }
}
