using CNull.Common.Events.Args;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors;
using CNull.ErrorHandler.Errors.Source;
using CNull.Source.Tests.Helpers;

namespace CNull.Source.Tests
{
    public class RawCodeSourceTests(CodeSourceDependenciesFixture dependenciesFixture) : IClassFixture<CodeSourceDependenciesFixture>
    {
        [Theory, ClassData(typeof(StreamBufferReadsData))]
        public void CanAdvanceToNextCharacter(string testBuffer, int numberOfReads)
        {
            // Arrange

            dependenciesFixture.Reset();
            dependenciesFixture.InputRepositoryMock.SetupGet(s => s.IsInitialized).Returns(true);
            dependenciesFixture.MockedBuffer = testBuffer;

            var codeInput = new RawCodeSource(
                dependenciesFixture.InputRepositoryMock.Object, 
                null!, 
                dependenciesFixture.MediatorMock.Object);
            var results = new List<char?>();

            // Act

            for (var i = 0; i < numberOfReads; i++)
            {
                codeInput.MoveToNext();
                results.Add(codeInput.CurrentCharacter);
            }

            // Assert

            Assert.Equal(numberOfReads, results.Count);
            Assert.Equivalent(dependenciesFixture.GetExpectedStreamReads(testBuffer, numberOfReads), results);
        }

        [Theory, ClassData(typeof(StreamBufferReadsData))]
        public void CanReadCurrentCharacter(string testBuffer, int numberOfReads)
        {
            // Arrange

            dependenciesFixture.Reset();
            dependenciesFixture.InputRepositoryMock.SetupGet(s => s.IsInitialized).Returns(true);
            dependenciesFixture.MockedBuffer = testBuffer;

            var codeInput = new RawCodeSource(
                dependenciesFixture.InputRepositoryMock.Object, 
                null!, 
                dependenciesFixture.MediatorMock.Object);

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

            dependenciesFixture.Reset();
            dependenciesFixture.InputRepositoryMock.SetupGet(s => s.IsInitialized).Returns(true);

            var codeInput = new RawCodeSource(
                dependenciesFixture.InputRepositoryMock.Object, 
                null!, 
                dependenciesFixture.MediatorMock.Object);

            // Act

            var character = codeInput.CurrentCharacter;

            // Assert

            Assert.Null(character);
        }

        [Fact]
        public void RaisesAnErrorWhenStreamIsNotInitialized()
        {
            // Arrange

            dependenciesFixture.Reset();
            dependenciesFixture.InputRepositoryMock.SetupGet(s => s.IsInitialized).Returns(false);

            var errorHandlerMock = new Mock<IErrorHandler>();
            errorHandlerMock.Setup(e => e.RaiseSourceError(It.IsAny<ISourceError>()));

            var codeInput = new RawCodeSource(
                dependenciesFixture.InputRepositoryMock.Object, 
                errorHandlerMock.Object, 
                dependenciesFixture.MediatorMock.Object);

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

            dependenciesFixture.Reset();
            dependenciesFixture.InputRepositoryMock.Setup(r => r.SetupFileStream(It.IsAny<string>()));
            dependenciesFixture.InputRepositoryMock.SetupGet(r => r.IsInitialized).Returns(true);

            const string testPath = "Sample path";
            var codeInput = new RawCodeSource(
                dependenciesFixture.InputRepositoryMock.Object, 
                null!,
                dependenciesFixture.MediatorMock.Object);

            // Act

            dependenciesFixture.MediatorMock.Raise(m => m.FileInputRequested += null,
                new FileInputRequestedEventArgs(testPath));

            // Assert

            dependenciesFixture.InputRepositoryMock.Verify(r => r.SetupFileStream(testPath), Times.Once);
        }

        [Fact]
        public void RaisesErrorWhenFileSetupThrowsException()
        {
            // Arrange

            dependenciesFixture.Reset();
            dependenciesFixture.InputRepositoryMock.
                Setup(r => r.SetupFileStream(It.IsAny<string>()))
                .Throws<IOException>();
            dependenciesFixture.InputRepositoryMock.SetupGet(r => r.IsInitialized).Returns(false);

            var errorHandlerMock = new Mock<IErrorHandler>();
            errorHandlerMock.Setup(e => e.RaiseSourceError(It.IsAny<ISourceError>()));

            const string testPath = "Sample path";
            var codeInput = new RawCodeSource(
                dependenciesFixture.InputRepositoryMock.Object, 
                errorHandlerMock.Object,
                dependenciesFixture.MediatorMock.Object);

            // Act

            dependenciesFixture.MediatorMock.Raise(m => m.FileInputRequested += null,
                new FileInputRequestedEventArgs(testPath));

            // Assert

            errorHandlerMock.Verify(e => e.RaiseSourceError(
                It.Is<FileAccessError>(error => error.FilePath == new FileAccessError(testPath).FilePath)), Times.Once);
        }
    }
}