using CNull.Lexer.States;
using CNull.Lexer.Tests.Data;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests.States
{
    public class StringLexerStateTests(LexerStateFixture fixture) : IClassFixture<LexerStateFixture>
    {
        [Theory, ClassData(typeof(StringLiteralsData))]
        public void CanBuildStringLiterals(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new StringLiteralLexerState(fixture.ServicesContainerMock.Object), fixture);

        [Theory, ClassData(typeof(StringLiteralsFinishedCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => StateTestsCore.TestFinishedCharacter(input, expectedCharacter,
                new StringLiteralLexerState(fixture.ServicesContainerMock.Object), fixture);
    }
}
