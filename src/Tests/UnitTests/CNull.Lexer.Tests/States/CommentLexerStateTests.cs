using CNull.Lexer.States;
using CNull.Lexer.Tests.Data;
using CNull.Lexer.Tests.Fixtures;

namespace CNull.Lexer.Tests.States
{
    public class CommentLexerStateTests(LexerStateFixture fixture) : IClassFixture<LexerStateFixture>
    {
        [Theory, ClassData(typeof(CommentsData))]
        public void CanBuildComments(string input, bool expectedResult, Token expectedToken)
            => StateTestsCore.TestTokensCreation(input, expectedResult, expectedToken,
                new CommentLexerState(fixture.ServicesContainerMock.Object), fixture);

        [Theory, ClassData(typeof(CommentsNextCharacterData))]
        public void CanFinishAtProperCharacter(string input, char? expectedCharacter)
            => StateTestsCore.TestFinishedCharacter(input, expectedCharacter,
                new CommentLexerState(fixture.ServicesContainerMock.Object), fixture);
    }
}
