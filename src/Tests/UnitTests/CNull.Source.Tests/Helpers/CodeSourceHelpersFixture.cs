using CNull.Common;
using CNull.Common.Configuration;

namespace CNull.Source.Tests.Helpers
{
    public class CodeSourceHelpersFixture : BaseSourceHelpersFixture
    {
        public Mock<IRawCodeSource> CodeSourceMock { get; private set; } = new();
        public Mock<ICNullConfiguration> CompilerConfigurationMock { get; private set; } = new();

        private char? _currentCharacter;

        //public override void Reset()
        //{
        //    base.Reset();
        //    FirstRead = false;

        //    CodeSourceMock = new Mock<IRawCodeSource>();
        //    ErrorHandlerMock = new Mock<IErrorHandler>();
        //    CompilerConfigurationMock = new Mock<ICNullConfiguration>();

        //    CodeSourceMock.Setup(s => s.MoveToNext()).Callback(() =>
        //    {
        //        FirstRead = true;
        //        AdvanceStream();
        //    });
        //    CodeSourceMock.SetupGet(s => s.CurrentCharacter)
        //        .Returns(() => !EndOfBuffer ? MockedBuffer[CurrentPosition] : null);
        //    CodeSourceMock.SetupGet(s => s.IsCurrentCharacterNewLine)
        //        .Returns(() => EndOfBuffer || MockedBuffer[CurrentPosition] == '\n');
        //    CodeSourceMock.SetupGet(s => s.Position)
        //        .Returns(() => FirstRead ? Position.FirstCharacter : default);
        //}

        public override void Setup(string buffer)
        {
            base.Setup(buffer);

            _currentCharacter = null;
            CodeSourceMock = new Mock<IRawCodeSource>();
            CompilerConfigurationMock = new Mock<ICNullConfiguration>();

            CodeSourceMock.Setup(s => s.MoveToNext()).Callback(() => SetCurrentCharacter(Reader.Read()));
            CodeSourceMock.SetupGet(s => s.CurrentCharacter)
                .Returns(() => _currentCharacter);
            CodeSourceMock.SetupGet(s => s.IsCurrentCharacterNewLine).Returns(() => _currentCharacter == '\n');
            CodeSourceMock.SetupGet(s => s.Position)
                .Returns(() => _currentCharacter != null ? Position.FirstCharacter : default);
        }

        public override IEnumerable<char?> GetExpectedStreamReads(string buffer, int numberOfReads)
            => base.GetExpectedStreamReads(buffer, numberOfReads).Where(c => c != '\r');

        private void SetCurrentCharacter(int valueFromBuffer)
            => _currentCharacter = valueFromBuffer != -1 ? (char)valueFromBuffer : null;
    }
}
