using CNull.Lexer.States;
using CNull.Lexer.Tests.Data;
using CNull.Source.Tests.Helpers;

namespace CNull.Lexer.Tests.States
{
    public class NumericLexerStateTests(NewLineProxyFixture fixture) : IClassFixture<NewLineProxyFixture>
    {
        [Theory, ClassData(typeof(IntegerLiteralsData))]
        public void CanBuildIntegerLiterals(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new NumericLexerState(fixture.CodeSourceMock.Object), fixture);

        [Theory, ClassData(typeof(FloatingPointLiteralsData))]
        public void CanBuildFloatingPointLiterals(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new NumericLexerState(fixture.CodeSourceMock.Object), fixture);

        [Theory, ClassData(typeof(NumericStateFinishingCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => StateTestsCore.TestFinishedCharacter(input, expectedCharacter,
                new NumericLexerState(fixture.CodeSourceMock.Object), fixture);
    }
}
