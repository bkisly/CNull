using CNull.Common;
using CNull.Common.Configuration;
using CNull.ErrorHandler;

namespace CNull.Source.Tests.Helpers
{
    public class NewLineProxyFixture : SourceFixture
    {
        public Mock<IRawCodeSource> CodeSourceMock { get; private set; } = new();
        public Mock<IErrorHandler> ErrorHandlerMock { get; private set; } = new();
        public Mock<ICompilerConfiguration> CompilerConfigurationMock { get; private set; } = new();

        public override void Reset()
        {
            base.Reset();
            FirstRead = false;

            CodeSourceMock = new Mock<IRawCodeSource>();
            ErrorHandlerMock = new Mock<IErrorHandler>();
            CompilerConfigurationMock = new Mock<ICompilerConfiguration>();

            CodeSourceMock.Setup(s => s.MoveToNext()).Callback(() =>
            {
                FirstRead = true;
                AdvanceStream();
            });
            CodeSourceMock.SetupGet(s => s.CurrentCharacter)
                .Returns(() => !EndOfBuffer ? MockedBuffer[CurrentPosition] : null);
            CodeSourceMock.SetupGet(s => s.IsCurrentCharacterNewLine)
                .Returns(() => EndOfBuffer || MockedBuffer[CurrentPosition] == '\n');
            CodeSourceMock.SetupGet(s => s.Position)
                .Returns(() => FirstRead ? Position.FirstCharacter : default);
        }

        public override IEnumerable<char?> GetExpectedStreamReads(string buffer, int numberOfReads)
            => base.GetExpectedStreamReads(buffer, numberOfReads).Where(c => c != '\r');
    }
}
