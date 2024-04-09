using CNull.Lexer.ServicesContainers;
using CNull.Source.Tests.Helpers;

namespace CNull.Lexer.Tests.Fixtures
{
    public sealed class LexerStateFixture : NewLineProxyFixture
    {
        public Mock<ILexerStateServicesContainer> ServicesContainerMock { get; private set; } = new();

        public LexerStateFixture()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();

            CompilerConfigurationMock.SetupGet(c => c.MaxCommentLength).Returns(1000);
            CompilerConfigurationMock.SetupGet(c => c.MaxIdentifierLength).Returns(1000);
            CompilerConfigurationMock.SetupGet(c => c.MaxStringLiteralLength).Returns(1000);
            CompilerConfigurationMock.SetupGet(c => c.MaxTokenLength).Returns(1000);
            CompilerConfigurationMock.SetupGet(c => c.MaxWhitespaceLength).Returns(1000);

            ServicesContainerMock = new Mock<ILexerStateServicesContainer>();
            ServicesContainerMock.SetupGet(s => s.ErrorHandler).Returns(ErrorHandlerMock.Object);
            ServicesContainerMock.SetupGet(s => s.CodeSource).Returns(CodeSourceMock.Object);
            ServicesContainerMock.SetupGet(s => s.CompilerConfiguration).Returns(CompilerConfigurationMock.Object);
        }
    }
}
