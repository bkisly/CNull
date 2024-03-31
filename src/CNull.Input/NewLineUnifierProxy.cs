namespace CNull.Input
{
    /// <summary>
    /// Class responsible for unifying new line representation to '\n' form. Proxy layer between lexer and input.
    /// </summary>
    internal class NewLineUnifierProxy(IRawCodeInput input) : ICodeInput
    {
        public char? CurrentCharacter => input.CurrentCharacter;

        public void MoveToNext()
        {
            input.MoveToNext();

            if(CurrentCharacter == '\r')
                input.MoveToNext();
        }
    }
}
