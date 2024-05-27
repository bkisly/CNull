using System.Text;
using CNull.Common;
using CNull.Common.Events;
using CNull.Source.Errors;
using CNull.Source.Tests.Data;
using CNull.Source.Tests.Fixtures;

namespace CNull.Source.Tests
{
    public class RawCodeSourceTests(RawSourceHelpersFixture fixture) : IClassFixture<RawSourceHelpersFixture>
    {
        [Theory, ClassData(typeof(StreamBufferReadsData))]
        public void CanAdvanceToNextCharacter(string testBuffer, int numberOfReads)
        {
            // Arrange

            fixture.Setup(testBuffer);

            var codeInput = new RawCodeSource(fixture.Reader, fixture.ErrorHandlerMock.Object,
                fixture.StateManagerMock.Object);
            var results = new List<char?>();

            // Act

            for (var i = 0; i < numberOfReads; i++)
            {
                codeInput.MoveToNext();
                results.Add(codeInput.CurrentCharacter);
            }

            // Assert

            Assert.Equal(numberOfReads, results.Count);
            Assert.Equivalent(fixture.GetExpectedStreamReads(testBuffer, numberOfReads), results);
        }

        [Theory, ClassData(typeof(StreamBufferReadsData))]
        public void CanReadCurrentCharacter(string testBuffer, int numberOfReads)
        {
            // Arrange

            fixture.Setup(testBuffer);

            var codeInput = new RawCodeSource(fixture.Reader, fixture.ErrorHandlerMock.Object,
                fixture.StateManagerMock.Object);

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

            fixture.Setup(string.Empty);

            var codeInput = new RawCodeSource(fixture.Reader, fixture.ErrorHandlerMock.Object,
                fixture.StateManagerMock.Object);

            // Act

            var character = codeInput.CurrentCharacter;

            // Assert

            Assert.Null(character);
        }

        [Fact]
        public void RaisesAnErrorWhenStreamIsNotInitialized()
        {
            // Arrange

            fixture.Setup();
            var codeInput = new RawCodeSource(fixture.ErrorHandlerMock.Object, fixture.StateManagerMock.Object);

            // Act

            codeInput.MoveToNext();
            var character = codeInput.CurrentCharacter;

            // Assert

            Assert.Null(character);
            fixture.ErrorHandlerMock.Verify(e => e.RaiseSourceError(It.IsAny<StreamNotInitializedError>()), Times.Once);
        }

        [Fact]
        public void CanInitializeFileInputWhenRequested()
        {
            // Arrange

            fixture.Setup();

            const string testPath = "Sample path";
            var codeInput = new RawCodeSource(fixture.ErrorHandlerMock.Object, 
                fixture.StateManagerMock.Object);

            // Act

            fixture.StateManagerMock.Raise(m => m.InputRequested += null,
                new InputRequestedEventArgs(new Lazy<Stream>(() => new MemoryStream("ABCDE"u8.ToArray())), testPath));

            // Assert

            Assert.Equal('A', codeInput.CurrentCharacter);
        }

        [Fact]
        public void RaisesErrorWhenFileSetupThrowsException()
        {
            // Arrange

            fixture.Setup();

            var failingStream = new Lazy<Stream>(() => throw new IOException());
            const string testPath = "Sample path";

            var codeInput = new RawCodeSource(fixture.ErrorHandlerMock.Object, fixture.StateManagerMock.Object);

            // Act

            fixture.StateManagerMock.Raise(m => m.InputRequested += null,
                new InputRequestedEventArgs(failingStream, testPath));

            // Assert

            Assert.Null(codeInput.CurrentCharacter);
            fixture.ErrorHandlerMock.Verify(e => e.RaiseSourceError(
                It.Is<InputAccessError>(error => error == new InputAccessError(testPath))), Times.Once);
        }

        [Theory, ClassData(typeof(PositionCounterData))]
        public void CanCountPosition(string input, int numberOfReads, Position expectedPosition)
        {
            // Arrange

            fixture.Setup(input);
            var source = new RawCodeSource(fixture.Reader, fixture.ErrorHandlerMock.Object,
                fixture.StateManagerMock.Object);

            // Act

            for (var i = 0; i < numberOfReads; i++)
                source.MoveToNext();

            // Assert

            Assert.Equal(expectedPosition, source.Position);
        }
    }
}