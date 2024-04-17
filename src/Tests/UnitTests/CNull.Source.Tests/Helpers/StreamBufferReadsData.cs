namespace CNull.Source.Tests.Helpers
{
    internal class StreamBufferReadsData : TheoryData<string, int>
    {
        public StreamBufferReadsData()
        {
            Add("Sample buffer", 5);
            Add("Sample buffer\n\n\r\nzażółć gęślą jaźń", 40);
            Add("testtest", 8);
            Add("", 10);
            Add("\n\r\n\r\r\r\nabcdefgh\r", 30);
            Add("      whitespace\r\r\n", 30);
            Add("\r\r\r\r\r\r\r\r\r", 6);
            Add("\r\r\rinner text\r\r\r\n\r", 15);
        }
    }
}
