using CNull.Common.Mediators;
using CNull.Source.Repositories;

namespace CNull.Source.Tests.Helpers
{
    public class CodeSourceDependenciesFixture : SourceFixture
    {
        public Mock<IInputRepository> InputRepositoryMock { get; private set; } = new();
        public Mock<ICoreComponentsMediator> MediatorMock { get; private set; } = new();

        public CodeSourceDependenciesFixture()
        {
            Reset();
        }

        public sealed override void Reset()
        {
            InputRepositoryMock = new Mock<IInputRepository>();
            InputRepositoryMock.Setup(s => s.Read())
                .Returns(() => !EndOfBuffer ? MockedBuffer[CurrentPosition] : -1)
                .Callback(AdvanceStream);

            MediatorMock = new Mock<ICoreComponentsMediator>();
            base.Reset();
            CurrentPosition = 0;
        }
    }
}
