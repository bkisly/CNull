using CNull.Source.Tests.Helpers;

namespace CNull.Source.Tests
{
    public class NewLineUnifierProxyTests(CodeSourceHelpersFixture sourceFixture) : IClassFixture<CodeSourceHelpersFixture>
    {
        [Theory, ClassData(typeof(StreamBufferReadsData))]
        public void CanReadAndFilterCrCharacters(string testBuffer, int numberOfReads)
        {
            // Arrange

            sourceFixture.Setup(testBuffer);

            var results = new List<char?>();
            var proxy = new NewLineUnifierProxy(sourceFixture.CodeSourceMock.Object);
            var expectedResults = sourceFixture.GetExpectedStreamReads(testBuffer, numberOfReads).ToArray();

            // Act

            for (var i = 0; i < numberOfReads; i++)
            {
                proxy.MoveToNext();
                results.Add(proxy.CurrentCharacter);
            }

            // Assert

            Assert.Equivalent(expectedResults, results);
        }
    }
}
