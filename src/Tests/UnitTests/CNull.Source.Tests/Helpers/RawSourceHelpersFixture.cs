using CNull.Common.Mediators;

namespace CNull.Source.Tests.Helpers
{
    public class RawSourceHelpersFixture : BaseSourceHelpersFixture
    {
        public Mock<ICoreComponentsMediator> MediatorMock { get; private set; } = new();

        public override void Setup(string buffer)
        {
            base.Setup(buffer);
            Setup();
        }

        public void Setup()
        {
            MediatorMock = new Mock<ICoreComponentsMediator>();
        }
    }
}
