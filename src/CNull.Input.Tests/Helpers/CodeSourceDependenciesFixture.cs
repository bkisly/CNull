using CNull.Common.Mediators;
using CNull.Source.Repositories;

namespace CNull.Source.Tests.Helpers
{
    public class CodeSourceDependenciesFixture
    {
        public Mock<IInputRepository> InputRepositoryMock { get; private set; } = new();
        public Mock<ICoreComponentsMediator> MediatorMock { get; private set; } = new();

        public string MockedBuffer { get; set; } = string.Empty;

        private bool EndOfStream => _currentPosition >= MockedBuffer.Length;
        private int _currentPosition;

        public CodeSourceDependenciesFixture()
        {
            Reset();
        }

        public void Reset()
        {
            InputRepositoryMock = new Mock<IInputRepository>();
            InputRepositoryMock.Setup(s => s.Read())
                .Returns(() => !EndOfStream ? MockedBuffer[_currentPosition] : -1)
                .Callback(AdvanceStream);

            MediatorMock = new Mock<ICoreComponentsMediator>();
            MockedBuffer = string.Empty;
            _currentPosition = 0;
        }

        public static IEnumerable<char?> GetExpectedStreamReads(string buffer, int numberOfReads)
        {
            var currentRead = 0;

            for (; currentRead < Math.Min(numberOfReads, buffer.Length); currentRead++)
                yield return buffer[currentRead++];

            for (; currentRead < numberOfReads; currentRead++)
                yield return null;
        }

        private void AdvanceStream()
        {
            if (EndOfStream)
                _currentPosition = MockedBuffer.Length;
            else _currentPosition++;
        }
    }
}
