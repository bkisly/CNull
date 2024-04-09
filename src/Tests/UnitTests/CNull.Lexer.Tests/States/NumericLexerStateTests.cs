using CNull.Lexer.States;
using CNull.Lexer.Tests.Data;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests.States
{
    public class NumericLexerStateTests(LexerStateFixture fixture) : IClassFixture<LexerStateFixture>
    {
        [Theory, ClassData(typeof(IntegerLiteralsData))]
        public void CanBuildIntegerLiterals(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new NumericLexerState(fixture.ServicesContainerMock.Object), fixture);

        [Theory, ClassData(typeof(FloatingPointLiteralsData))]
        public void CanBuildFloatingPointLiterals(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new NumericLexerState(fixture.ServicesContainerMock.Object), fixture);

        [Theory, ClassData(typeof(NumericStateFinishingCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => StateTestsCore.TestFinishedCharacter(input, expectedCharacter,
                new NumericLexerState(fixture.ServicesContainerMock.Object), fixture);
    }
}
