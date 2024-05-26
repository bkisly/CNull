using CNull.Common.State;

namespace CNull.Source.Tests.Fixtures
{
    public class RawSourceHelpersFixture : BaseSourceHelpersFixture
    {
        public Mock<IStateManager> StateManagerMock { get; private set; } = new();

        public override void Setup(string buffer)
        {
            base.Setup(buffer);
            Setup();
        }

        public void Setup()
        {
            StateManagerMock = new Mock<IStateManager>();
        }
    }
}
