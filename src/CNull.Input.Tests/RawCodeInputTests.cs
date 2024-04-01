using CNull.Source.Raw;
using CNull.Source.Tests.Helpers;

namespace CNull.Source.Tests
{
    public class RawCodeInputTests(RepositoryFixture repositoryFixture) : IClassFixture<RepositoryFixture>
    {
        [Theory, ClassData(typeof(StreamBufferReadsData))]
        public void CanAdvanceToNextCharacter(string testBuffer, int numberOfReads)
        {
            // Arrange

            repositoryFixture.Reset();
            repositoryFixture.InputRepositoryMock.SetupGet(s => s.IsInitialized).Returns(true);
            repositoryFixture.MockedBuffer = testBuffer;

            var codeInput = new RawCodeInput(repositoryFixture.InputRepositoryMock.Object);
            var results = new List<char?>();

            // Act

            for (var i = 0; i < numberOfReads; i++)
            {
                codeInput.MoveToNext();
                results.Add(codeInput.CurrentCharacter);
            }

            // Assert

            Assert.Equal(numberOfReads, results.Count);
            Assert.Equivalent(RepositoryFixture.GetExpectedStreamReads(testBuffer, numberOfReads), results);
        }

        [Theory, ClassData(typeof(StreamBufferReadsData))]
        public void CanReadCurrentCharacter(string testBuffer, int numberOfReads)
        {
            // Arrange

            repositoryFixture.Reset();
            repositoryFixture.InputRepositoryMock.SetupGet(s => s.IsInitialized).Returns(true);
            repositoryFixture.MockedBuffer = testBuffer;

            var codeInput = new RawCodeInput(repositoryFixture.InputRepositoryMock.Object);

            // Act

            for (var i = 0; i < numberOfReads; i++)
                codeInput.MoveToNext();

            var character = codeInput.CurrentCharacter;

            // Assert

            Assert.Equal(numberOfReads <= testBuffer.Length ? testBuffer[numberOfReads - 1] : null, character);
        }

        [Fact]
        public void CanReadCurrentCharacterWithoutAdvancing()
        {
            // Arrange

            repositoryFixture.Reset();
            repositoryFixture.InputRepositoryMock.SetupGet(s => s.IsInitialized).Returns(true);

            var codeInput = new RawCodeInput(repositoryFixture.InputRepositoryMock.Object);

            // Act

            var character = codeInput.CurrentCharacter;

            // Assert

            Assert.Null(character);
        }
    }
}