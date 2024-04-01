namespace CNull.Source.Proxies
{
    /// <summary>
    /// Class responsible for unifying new line representation to '\n' form. Proxy layer between lexer and input.
    /// </summary>
    public class NewLineUnifierProxy(IRawCodeSource source) : ICodeSource
    {
        public char? CurrentCharacter => source.CurrentCharacter;

        public void MoveToNext()
        {
            source.MoveToNext();

            if (CurrentCharacter == '\r')
                source.MoveToNext();
        }
    }
}
