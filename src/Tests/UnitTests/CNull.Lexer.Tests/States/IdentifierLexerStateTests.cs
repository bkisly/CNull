﻿using CNull.Lexer.States;
using CNull.Lexer.Tests.Helpers;
using CNull.Source.Tests.Helpers;

namespace CNull.Lexer.Tests.States
{
    public class IdentifierLexerStateTests(NewLineProxyFixture fixture) : IClassFixture<NewLineProxyFixture>
    {
        [Theory, ClassData(typeof(IdentifiersData))]
        public void CanBuildIdentifiers(string input, bool succeeded, Token token)
            => CanBuildIdentifiersOrKeywords(input, succeeded, token);

        [Theory, ClassData(typeof(KeywordsData))]
        public void CanBuildKeywords(string input, bool succeeded, Token token)
            => CanBuildIdentifiersOrKeywords(input, succeeded, token);

        private void CanBuildIdentifiersOrKeywords(string input, bool succeeded, Token token)
        {
            // Arrange

            fixture.Reset();
            fixture.MockedBuffer = input;
            fixture.CodeSourceMock.Object.MoveToNext();

            var state = new IdentifierLexerState(fixture.CodeSourceMock.Object);

            // Act

            var result = state.TryBuildToken(out var resultToken);

            // Assert

            Assert.Equal(succeeded, result);
            Assert.Equivalent(token, resultToken);
        }
    }
}
