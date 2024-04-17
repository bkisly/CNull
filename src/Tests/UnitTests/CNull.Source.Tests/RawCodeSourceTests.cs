using CNull.Common;
using CNull.Common.Events.Args;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors;
using CNull.ErrorHandler.Errors.Source;
using CNull.Source.Tests.Helpers;

namespace CNull.Source.Tests
{
    public class RawCodeSourceTests(CodeSourceFixture fixture) : IClassFixture<CodeSourceFixture>
    {
        [Theory, ClassData(typeof(StreamBufferReadsData))]
        public void CanAdvanceToNextCharacter(string testBuffer, int numberOfReads)
        {
            // Arrange

            fixture.Reset();
            fixture.InputRepositoryMock.SetupGet(s => s.IsInitialized).Returns(true);
            fixture.MockedBuffer = testBuffer;

            var codeInput = new RawCodeSource(
                fixture.InputRepositoryMock.Object, 
                null!, 
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

            fixture.Reset();
            fixture.InputRepositoryMock.SetupGet(s => s.IsInitialized).Returns(true);
            fixture.MockedBuffer = testBuffer;

            var codeInput = new RawCodeSource(
                fixture.InputRepositoryMock.Object, 
                null!, 
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

            fixture.Reset();
            fixture.InputRepositoryMock.SetupGet(s => s.IsInitialized).Returns(true);

            var codeInput = new RawCodeSource(
                fixture.InputRepositoryMock.Object, 
                null!, 
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

            fixture.Reset();
            fixture.InputRepositoryMock.SetupGet(s => s.IsInitialized).Returns(false);

            var errorHandlerMock = new Mock<IErrorHandler>();
            errorHandlerMock.Setup(e => e.RaiseSourceError(It.IsAny<ISourceError>()));

            var codeInput = new RawCodeSource(
                fixture.InputRepositoryMock.Object, 
                errorHandlerMock.Object, 
                fixture.MediatorMock.Object);

            // Act

            codeInput.MoveToNext();
            var character = codeInput.CurrentCharacter;

            // Assert

            Assert.Null(character);
            errorHandlerMock.Verify(e => e.RaiseSourceError(It.IsAny<StreamNotInitializedError>()), Times.Once);
        }

        [Fact]
        public void CanInitializeFileInputWhenRequested()
        {
            // Arrange

            fixture.Reset();
            fixture.InputRepositoryMock.Setup(r => r.SetupFileStream(It.IsAny<string>()));
            fixture.InputRepositoryMock.SetupGet(r => r.IsInitialized).Returns(true);

            const string testPath = "Sample path";
            var codeInput = new RawCodeSource(
                fixture.InputRepositoryMock.Object, 
                null!,
                fixture.MediatorMock.Object);

            // Act

            fixture.MediatorMock.Raise(m => m.FileInputRequested += null,
                new FileInputRequestedEventArgs(testPath));

            // Assert

            fixture.InputRepositoryMock.Verify(r => r.SetupFileStream(testPath), Times.Once);
        }

        [Fact]
        public void RaisesErrorWhenFileSetupThrowsException()
        {
            // Arrange

            fixture.Reset();
            fixture.InputRepositoryMock.
                Setup(r => r.SetupFileStream(It.IsAny<string>()))
                .Throws<IOException>();
            fixture.InputRepositoryMock.SetupGet(r => r.IsInitialized).Returns(false);

            var errorHandlerMock = new Mock<IErrorHandler>();
            errorHandlerMock.Setup(e => e.RaiseSourceError(It.IsAny<ISourceError>()));

            const string testPath = "Sample path";
            var codeInput = new RawCodeSource(
                fixture.InputRepositoryMock.Object, 
                errorHandlerMock.Object,
                fixture.MediatorMock.Object);

            // Act

            fixture.MediatorMock.Raise(m => m.FileInputRequested += null,
                new FileInputRequestedEventArgs(testPath));

            // Assert

            errorHandlerMock.Verify(e => e.RaiseSourceError(
                It.Is<FileAccessError>(error => error.FilePath == new FileAccessError(testPath).FilePath)), Times.Once);
        }

        [Theory, ClassData(typeof(PositionCounterData))]
        public void CanCountPosition(string input, int numberOfReads, Position expectedPosition)
        {
            // Arrange

            var reader = new StringReader(input);
            fixture.Reset();
            fixture.InputRepositoryMock.Setup(r => r.Read()).Returns(reader.Read);
            fixture.InputRepositoryMock.SetupGet(r => r.IsInitialized).Returns(true);

            var source = new RawCodeSource(fixture.InputRepositoryMock.Object, new Mock<IErrorHandler>().Object, fixture.MediatorMock.Object);

            // Act

            for (var i = 0; i < numberOfReads; i++)
                source.MoveToNext();

            // Assert

            Assert.Equal(expectedPosition, source.Position);
        }
    }
}