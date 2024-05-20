namespace CNull.Common.Configuration
{
    public class InMemoryCNullConfiguration : ICNullConfiguration
    {
        public int MaxStringLiteralLength => 65536;
        public int MaxCommentLength => 65536;
        public int MaxIdentifierLength => 256;
        public int MaxWhitespaceLength => 65536;
        public int MaxTokenLength => 65536;
        public int MaxErrorsCount => 64;
    }
}
