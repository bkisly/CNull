using CNull.Parser;

namespace CNull.Interpreter
{
    public class Interpreter(IParser parser) : IInterpreter
    {
        public void Execute()
        {
            parser.Parse();
            throw new NotImplementedException();
        }
    }
}
