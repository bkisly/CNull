using CNull.Common;
using CNull.Parser;
using CNull.Parser.Productions;

namespace CNull.Interpreter.Symbols.StandardLibrary
{
    public class StandardLibrary
    {
        private readonly StandardInput _standardInput;
        private readonly StandardOutput _standardOutput;

        private readonly Dictionary<string, StandardLibraryFunction> _functions;

        public StandardLibrary(StandardInput standardInput, StandardOutput standardOutput)
        {
            _standardInput = standardInput;
            _standardOutput = standardOutput;

            _functions = new Dictionary<string, StandardLibraryFunction>
            {
                [nameof(WriteLine)] = new(
                    new ReturnType(new Position()),
                    nameof(WriteLine),
                    [
                        new Parameter(
                            new PrimitiveType(PrimitiveTypes.String, new Position()), "content", new Position()
                        )
                    ],
                    WriteLine),

                [nameof(ReadLine)] = new(
                    new PrimitiveType(PrimitiveTypes.String, new Position()),
                    nameof(WriteLine),
                    [],
                    ReadLine),

                [nameof(ConvertToInt)] = new(
                    new PrimitiveType(PrimitiveTypes.Integer, new Position()),
                    nameof(ConvertToInt),
                    [
                        new Parameter(
                            new PrimitiveType(PrimitiveTypes.String, new Position()), "text", new Position()
                        )
                    ],
                    ConvertToInt),
            };
        }

        private object? WriteLine(object?[] args)
        {
            _standardOutput.Invoke($"{args[0]}{Environment.NewLine}");
            return null;
        }

        private object? ReadLine(object?[] args)
        {
            var input = _standardInput.Invoke(string.Empty);
            _standardOutput.Invoke(Environment.NewLine);
            return input;
        }

        private object ConvertToInt(object?[] args)
        {
            return Convert.ToInt32(args[0]);
        }
    }
}
