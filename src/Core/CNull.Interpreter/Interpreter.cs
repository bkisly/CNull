using CNull.ErrorHandler;
using CNull.Parser;
using CNull.Parser.Visitors;

namespace CNull.Interpreter
{
    public class Interpreter(IParser parser, IErrorHandler errorHandler) : IInterpreter
    {
        public void Execute(Func<string, string?> inputCallback, Action<string> outputCallback)
        {
            var program = parser.Parse();
            var stringifier = new AstStringifierVisitor();

            if (errorHandler.Errors.Any())
                return;

            if (program != null)
                Console.WriteLine(stringifier.GetString(program));
        }
    }
}
