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
            CodeSourceMock = new Mock<IRawCodeSource>();
            CodeSourceMock.Setup(s => s.MoveToNext()).Callback(AdvanceStream);
            CodeSourceMock.SetupGet(s => s.CurrentCharacter)
                .Returns(() => !EndOfBuffer ? MockedBuffer[CurrentPosition] : null);

            base.Reset();
        }

        public override IEnumerable<char?> GetExpectedStreamReads(string buffer, int numberOfReads)
            => base.GetExpectedStreamReads(buffer, numberOfReads).Where(c => c != '\r');
    }
}
