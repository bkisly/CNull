using CNull.Lexer.States;
using CNull.Lexer.Tests.Data;
using CNull.Source.Tests.Helpers;

namespace CNull.Lexer.Tests.States
{
    public class StringLexerStateTests(NewLineProxyFixture fixture) : IClassFixture<NewLineProxyFixture>
    {
        [Theory, ClassData(typeof(StringLiteralsData))]
        public void CanBuildStringLiterals(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new StringLiteralLexerState(fixture.CodeSourceMock.Object, fixture.ErrorHandlerMock.Object, fixture.CompilerConfigurationMock.Object), fixture);

        [Theory, ClassData(typeof(StringLiteralsFinishedCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => StateTestsCore.TestFinishedCharacter(input, expectedCharacter,
                new StringLiteralLexerState(fixture.CodeSourceMock.Object, fixture.ErrorHandlerMock.Object, fixture.CompilerConfigurationMock.Object), fixture);
    }
}
