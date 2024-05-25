using CNull.Common;
using CNull.Common.Configuration;

namespace CNull.Source.Tests.Fixtures
{
    public class CodeSourceHelpersFixture : BaseSourceHelpersFixture
    {
        public Mock<IRawCodeSource> CodeSourceMock { get; private set; } = new();
        public Mock<ICNullConfiguration> CompilerConfigurationMock { get; private set; } = new();

        private char? _currentCharacter;
        private int _currentPosition;

        public override void Setup(string buffer)
        {
            base.Setup(buffer);

            _currentCharacter = null;
            _currentPosition = 0;

            CodeSourceMock = new Mock<IRawCodeSource>();
            CompilerConfigurationMock = new Mock<ICNullConfiguration>();

            CodeSourceMock.Setup(s => s.MoveToNext()).Callback(() =>
            {
                _currentPosition++;
                SetCurrentCharacter(Reader.Read());
            });
            CodeSourceMock.SetupGet(s => s.CurrentCharacter)
                .Returns(() => _currentCharacter);
            CodeSourceMock.SetupGet(s => s.IsCurrentCharacterNewLine).Returns(() => _currentCharacter == '\n');
            CodeSourceMock.SetupGet(s => s.Position)
                .Returns(() => _currentPosition == 0 ? new Position() : new Position(1, _currentPosition));
        }

        public override IEnumerable<char?> GetExpectedStreamReads(string buffer, int numberOfReads)
            => base.GetExpectedStreamReads(buffer, numberOfReads).Where(c => c != '\r');

        private void SetCurrentCharacter(int valueFromBuffer)
            => _currentCharacter = valueFromBuffer != -1 ? (char)valueFromBuffer : null;
    }
}
