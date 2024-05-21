using CNull.Common;
using CNull.Common.Events;
using CNull.ErrorHandler.Errors.Source;
using CNull.Source.Tests.Helpers;

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
                fixture.MediatorMock.Object);
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
                fixture.MediatorMock.Object);

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
                fixture.MediatorMock.Object);

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
            var codeInput = new RawCodeSource(fixture.ErrorHandlerMock.Object, fixture.MediatorMock.Object);

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
            var codeInput = new RawCodeSource(
                fixture.ErrorHandlerMock.Object, 
                fixture.MediatorMock.Object);

            // Act

            fixture.MediatorMock.Raise(m => m.InputRequested += null,
                new InputRequestedEventArgs(new Lazy<TextReader>(() => new StringReader("ABCDE")), testPath));

            // Assert

            Assert.Equal(codeInput.CurrentCharacter, 'A');
        }

        [Fact]
        public void RaisesErrorWhenFileSetupThrowsException()
        {
            // Arrange

            fixture.Setup();

            var failingReader = new Lazy<TextReader>(() => throw new IOException());
            const string testPath = "Sample path";

            var codeInput = new RawCodeSource(fixture.ErrorHandlerMock.Object, fixture.MediatorMock.Object);

            // Act

            fixture.MediatorMock.Raise(m => m.InputRequested += null,
                new InputRequestedEventArgs(failingReader, testPath));

            // Assert

            Assert.Null(codeInput.CurrentCharacter);
            fixture.ErrorHandlerMock.Verify(e => e.RaiseSourceError(
                It.Is<InputAccessError>(error => error.SourcePath == new InputAccessError(testPath).SourcePath)), Times.Once);
        }

        [Theory, ClassData(typeof(PositionCounterData))]
        public void CanCountPosition(string input, int numberOfReads, Position expectedPosition)
        {
            // Arrange

            fixture.Setup(input);
            var source = new RawCodeSource(fixture.Reader, fixture.ErrorHandlerMock.Object,
                fixture.MediatorMock.Object);

            // Act

            for (var i = 0; i < numberOfReads; i++)
                source.MoveToNext();

            // Assert

            Assert.Equal(expectedPosition, source.Position);
        }
    }
}