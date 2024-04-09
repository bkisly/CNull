using CNull.Lexer.States;
using CNull.Lexer.Tests.Data;
using CNull.Source.Tests.Helpers;

namespace CNull.Lexer.Tests.States
{
    public class CharLexerStateTests(NewLineProxyFixture fixture) : IClassFixture<NewLineProxyFixture>
    {
        [Theory, ClassData(typeof(CharLiteralsData))]
        public void CanBuildCharLiterals(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new CharLiteralLexerState(fixture.CodeSourceMock.Object), fixture);

        [Theory, ClassData(typeof(CharLiteralsFinishCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => StateTestsCore.TestFinishedCharacter(input, expectedCharacter,
                new CharLiteralLexerState(fixture.CodeSourceMock.Object), fixture);
    }
}
