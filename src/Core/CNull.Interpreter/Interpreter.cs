using CNull.Parser;

namespace CNull.Interpreter
{
    public class Interpreter(IParser parser) : IInterpreter
    {
        public void Execute(Func<string, string?> inputCallback, Action<string> outputCallback)
        {
            parser.Parse();
            //throw new NotImplementedException();
        }
    }
}
