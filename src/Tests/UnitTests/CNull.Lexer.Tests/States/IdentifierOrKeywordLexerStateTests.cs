using CNull.Lexer.States;
using CNull.Lexer.Tests.Data;
using CNull.Source.Tests.Helpers;

namespace CNull.Lexer.Tests.States
{
    public class IdentifierOrKeywordLexerStateTests(NewLineProxyFixture fixture) : IClassFixture<NewLineProxyFixture>
    {
        [Theory, ClassData(typeof(IdentifiersData))]
        public void CanBuildIdentifiers(string input, bool succeeded, Token token)
            => CanBuildIdentifiersOrKeywords(input, succeeded, token);

        [Theory, ClassData(typeof(KeywordsData))]
        public void CanBuildKeywords(string input, bool succeeded, Token token)
            => CanBuildIdentifiersOrKeywords(input, succeeded, token);

        [Theory, ClassData(typeof(IdentifiersLastCharacterData))]
        public void CanAdvanceToTheNextTokenCharacterWhenBuilt(string input, char? expectedCurrentCharacter)
        {
            // Arrange

            fixture.Reset();
            fixture.MockedBuffer = input;
            fixture.CodeSourceMock.Object.MoveToNext();

            var state = new IdentifierOrKeywordLexerState(fixture.CodeSourceMock.Object);

            // Act

            state.TryBuildToken(out _);

            // Assert

            Assert.Equal(expectedCurrentCharacter, fixture.CodeSourceMock.Object.CurrentCharacter);
        }

        private void CanBuildIdentifiersOrKeywords(string input, bool succeeded, Token token)
        {
            // Arrange

            fixture.Reset();
            fixture.MockedBuffer = input;
            fixture.CodeSourceMock.Object.MoveToNext();

            var state = new IdentifierOrKeywordLexerState(fixture.CodeSourceMock.Object);

            // Act

            var result = state.TryBuildToken(out var resultToken);

            // Assert

            Assert.Equal(succeeded, result);
            Assert.Equivalent(token, resultToken);
        }
    }
}
