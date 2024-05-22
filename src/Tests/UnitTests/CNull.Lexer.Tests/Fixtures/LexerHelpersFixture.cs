using CNull.Source.Tests.Fixtures;

namespace CNull.Lexer.Tests.Fixtures
{
    public class LexerHelpersFixture : CodeSourceHelpersFixture
    {
        public override void Setup(string buffer)
        {
            base.Setup(buffer);
            CodeSourceMock.Object.MoveToNext();

            CompilerConfigurationMock.SetupGet(c => c.MaxCommentLength).Returns(1000);
            CompilerConfigurationMock.SetupGet(c => c.MaxIdentifierLength).Returns(1000);
            CompilerConfigurationMock.SetupGet(c => c.MaxStringLiteralLength).Returns(1000);
            CompilerConfigurationMock.SetupGet(c => c.MaxTokenLength).Returns(1000);
            CompilerConfigurationMock.SetupGet(c => c.MaxWhitespaceLength).Returns(1000);
        }
    }
}
