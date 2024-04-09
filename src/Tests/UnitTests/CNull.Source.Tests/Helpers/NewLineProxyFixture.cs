using CNull.Common;

namespace CNull.Source.Tests.Helpers
{
    public class NewLineProxyFixture : SourceFixture
    {
        public Mock<IRawCodeSource> CodeSourceMock { get; private set; } = new();

        public NewLineProxyFixture()
        {
            Reset();
        }

        public sealed override void Reset()
        {
            base.Reset();
            FirstRead = false;

            CodeSourceMock = new Mock<IRawCodeSource>();
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
