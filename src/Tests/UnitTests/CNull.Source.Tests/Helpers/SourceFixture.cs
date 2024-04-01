namespace CNull.Source.Tests.Helpers
{
    public class SourceFixture
    {
        public string MockedBuffer { get; set; } = string.Empty;

        protected int CurrentPosition;
        protected bool EndOfBuffer => CurrentPosition >= MockedBuffer.Length;

        public virtual void Reset()
        {
            MockedBuffer = string.Empty;
            CurrentPosition = -1;
        }

        public virtual IEnumerable<char?> GetExpectedStreamReads(string buffer, int numberOfReads)
        {
            var currentRead = 0;

            for (; currentRead < Math.Min(numberOfReads, buffer.Length); currentRead++)
                yield return buffer[currentRead++];

            for (; currentRead < numberOfReads; currentRead++)
                yield return null;
        }

        protected void AdvanceStream()
        {
            if (EndOfBuffer)
                CurrentPosition = MockedBuffer.Length;
            else CurrentPosition++;
        }
    }
}