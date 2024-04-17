using CNull.Lexer.States;
using CNull.Lexer.Tests.Data;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests.States
{
    public class CharLexerStateTests(LexerStateFixture fixture) : IClassFixture<LexerStateFixture>
    {
        [Theory, ClassData(typeof(CharLiteralsData))]
        public void CanBuildCharLiterals(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new CharLiteralLexerState(fixture.ServicesContainerMock.Object), fixture);

        [Theory, ClassData(typeof(CharLiteralsFinishCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => StateTestsCore.TestFinishedCharacter(input, expectedCharacter,
                new CharLiteralLexerState(fixture.ServicesContainerMock.Object), fixture);
    }
}
