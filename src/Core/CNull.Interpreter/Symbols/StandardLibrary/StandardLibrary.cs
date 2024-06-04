using CNull.Common;
using CNull.Interpreter.Context;
using CNull.Interpreter.Errors;
using CNull.Interpreter.Extensions;
using CNull.Parser;
using CNull.Parser.Productions;

namespace CNull.Interpreter.Symbols.StandardLibrary
{
    public record StandardLibrarySignature(string SubmoduleName, string FunctionName);

    public class StandardLibrary
    {
        private readonly IExecutionEnvironment _environment;
        private readonly StandardInput _standardInput;
        private readonly StandardOutput _standardOutput;

        public const string CNullModule = "CNull";
        public Dictionary<StandardLibrarySignature, StandardLibraryFunction> StandardLibraryFunctions { get; }

        public StandardLibrary(IExecutionEnvironment environment, StandardInput standardInput, StandardOutput standardOutput)
        {
            _environment = environment;
            _standardInput = standardInput;
            _standardOutput = standardOutput;

            StandardLibraryFunctions = CreateStdlibFunctions();
        }

        private Dictionary<StandardLibrarySignature, StandardLibraryFunction> CreateStdlibFunctions()
        {
            return new Dictionary<StandardLibrarySignature, StandardLibraryFunction>
            {
                [new StandardLibrarySignature("Console", nameof(WriteLine))] = new(
                    new ReturnType(new Position()),
                    nameof(WriteLine),
                    [
                        new Parameter(
                            new PrimitiveType(PrimitiveTypes.String, new Position()), "content", new Position()
                        )
                    ],
                    WriteLine),

                [new StandardLibrarySignature("Console", nameof(Write))] = new(
                    new ReturnType(new Position()),
                    nameof(Write),
                    [
                        new Parameter(
                            new PrimitiveType(PrimitiveTypes.String, new Position()), "content", new Position()
                        )
                    ],
                    () => Write()),

                [new StandardLibrarySignature("Console", nameof(ReadLine))] = new(
                    new PrimitiveType(PrimitiveTypes.String, new Position()),
                    nameof(WriteLine),
                    [],
                    ReadLine),

                [new StandardLibrarySignature("Converters", nameof(StringToInt))] = new(
                    new PrimitiveType(PrimitiveTypes.Integer, new Position()),
                    nameof(StringToInt),
                    [
                        new Parameter(
                            new PrimitiveType(PrimitiveTypes.String, new Position()), "text", new Position()
                        )
                    ],
                    StringToInt),
            };
        }

        private void WriteLine() => Write(Environment.NewLine);

        private void Write(string trailingCharacter = "")
        {
            var content = _environment.CurrentContext.GetVariable("content");

            if (content.ValueContainer.Value is not { } stringValue)
            {
                _environment.ActiveException = RuntimeErrors.NullValueException;
                return;
            }

            _standardOutput.Invoke($"{stringValue}{trailingCharacter}");
        }

        private void ReadLine()
        {
            var input = _standardInput.Invoke();
            _environment.SaveResult(new ValueContainer(typeof(string).MakeNullableType(), input));
            _environment.CurrentContext.IsReturning = true;
        }

        private void StringToInt()
        {
            var text = _environment.CurrentContext.GetVariable("text");

            if (text.ValueContainer.Value is not { } stringValue)
            {
                _environment.ActiveException = RuntimeErrors.NullValueException;
                return;
            }

            var result = Convert.ToInt32(stringValue);

            _environment.SaveResult(new ValueContainer(typeof(int).MakeNullableType(), result));
            _environment.CurrentContext.IsReturning = true;
        }
    }
}
