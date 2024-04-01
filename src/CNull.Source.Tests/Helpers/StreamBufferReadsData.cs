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
        }
    }
}
