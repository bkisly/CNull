using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors;
using CNull.ErrorHandler.Exceptions;

namespace CNull.Source.Tests.Fixtures
{
    public class BaseSourceHelpersFixture
    {
        public StringReader Reader { get; protected set; } = null!;
        public Mock<IErrorHandler> ErrorHandlerMock { get; protected set; } = new();

        public virtual void Setup(string buffer)
        {
            ErrorHandlerMock = new Mock<IErrorHandler>();
            ErrorHandlerMock.Setup(e => e.RaiseSourceError(It.IsAny<ISourceError>())).Returns(new FatalErrorException(""));
            Reader = new StringReader(buffer);
        }

        public virtual IEnumerable<char?> GetExpectedStreamReads(string buffer, int numberOfReads)
        {
            var reader = new StringReader(buffer);
            for (var i = 0; i < numberOfReads; i++)
            {
                var readCharacter = reader.Read();
                yield return readCharacter != -1 ? (char)readCharacter : null;
            }
        }
    }
}