using System.Collections;
using CNull.Common;
using CNull.Interpreter.Context;
using CNull.Interpreter.Errors;
using CNull.Interpreter.Extensions;
using CNull.Interpreter.Resolvers;
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
        private readonly Dictionary<Func<object?, bool>, EmbeddedFunction> _embeddedFunctions;

        private ValueContainer _parentValueContainer = null!;

        public StandardLibrary(IExecutionEnvironment environment, StandardInput standardInput, StandardOutput standardOutput)
        {
            _environment = environment;
            _standardInput = standardInput;
            _standardOutput = standardOutput;

            StandardLibraryFunctions = CreateStdlibFunctions();
            _embeddedFunctions = CreateEmbeddedFunctions();
        }

        public EmbeddedFunction? GetEmbeddedFunction(string name, ValueContainer parentValue)
        {
            if (parentValue.Value == null)
            {
                _environment.ActiveException = RuntimeErrors.NullValueException;
                return null;
            }

            _parentValueContainer = parentValue;
            return _embeddedFunctions.FirstOrDefault(kvp => kvp.Key.Invoke(parentValue.Value) && kvp.Value.Name == name).Value;
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

        private Dictionary<Func<object?, bool>, EmbeddedFunction> CreateEmbeddedFunctions()
        {
            var embeddedFunctions = new Dictionary<Func<object?, bool>, EmbeddedFunction>
            {
                [value => value is string or null] = new(
                    new PrimitiveType(PrimitiveTypes.Char, new Position()),
                    "Get",
                    [
                        new Parameter(
                            new PrimitiveType(PrimitiveTypes.Integer, new Position()),
                            "index",
                            new Position())
                    ],
                    GetCharFromString),
            };

            foreach (var keyType in Enum.GetValues<PrimitiveTypes>())
            {
                foreach (var valueType in Enum.GetValues<PrimitiveTypes>())
                {
                    embeddedFunctions.Add(v => DictionaryTypePredicate(v, keyType, valueType), new EmbeddedFunction(
                        new PrimitiveType(valueType, new Position()),
                        "Get",
                        [
                            new Parameter(
                                new PrimitiveType(keyType, new Position()),
                                "key",
                                new Position()
                                )
                        ],
                        DictionaryGet
                        ));

                    embeddedFunctions.Add(v => DictionaryTypePredicate(v, keyType, valueType), new EmbeddedFunction(
                        new ReturnType(new Position()),
                        "Add",
                        [
                            new Parameter(
                                new PrimitiveType(keyType, new Position()),
                                "key",
                                new Position()
                            ),

                            new Parameter(
                                new PrimitiveType(keyType, new Position()),
                                "value",
                                new Position()
                            ),
                        ],
                        DictionaryAdd
                    ));

                    embeddedFunctions.Add(v => DictionaryTypePredicate(v, keyType, valueType), new EmbeddedFunction(
                        new ReturnType(new Position()),
                        "Set",
                        [
                            new Parameter(
                                new PrimitiveType(keyType, new Position()),
                                "key",
                                new Position()
                            ),

                            new Parameter(
                                new PrimitiveType(keyType, new Position()),
                                "value",
                                new Position()
                            ),
                        ],
                        DictionarySet
                    ));

                    embeddedFunctions.Add(v => DictionaryTypePredicate(v, keyType, valueType), new EmbeddedFunction(
                        new ReturnType(new Position()),
                        "Remove",
                        [
                            new Parameter(
                                new PrimitiveType(keyType, new Position()),
                                "key",
                                new Position()
                            ),
                        ],
                        DictionaryRemove
                    ));
                }
            }

            return embeddedFunctions;
        }

        private bool DictionaryTypePredicate(object? value, PrimitiveTypes keyType, PrimitiveTypes valueType)
        {
            if (value == null)
                return false;

            var type = value.GetType();
            if (!type.IsGenericType)
                return false;

            var realKeyType = TypesResolver.ResolvePrimitiveType(keyType);
            var realValueType = TypesResolver.ResolvePrimitiveType(valueType);
            var args = type.GetGenericArguments();
            return type.GetGenericTypeDefinition() == typeof(Dictionary<,>) && args[0] == realKeyType && args[1] == realValueType;
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

            _environment.SaveResult(new ValueContainer(typeof(int?), result));
            _environment.CurrentContext.IsReturning = true;
        }

        private void GetCharFromString()
        {
            if (_parentValueContainer.Value is not { } stringValue)
            {
                _environment.ActiveException = RuntimeErrors.NullValueException;
                return;
            }

            var index = _environment.CurrentContext.GetVariable("index");

            if (index.ValueContainer.Value is not { } indexValue)
            {
                _environment.ActiveException = RuntimeErrors.NullValueException;
                return;
            }

            var parentValue = stringValue.ToString()!;
            char character;

            try
            {
                character = parentValue[(int)indexValue];
            }
            catch (IndexOutOfRangeException)
            {
                _environment.ActiveException = RuntimeErrors.IndexOutOfRangeException;
                return;
            }

            _environment.SaveResult(new ValueContainer(typeof(char?), character));
            _environment.CurrentContext.IsReturning = true;
        }

        private void DictionaryGet()
        {
            if (ValidateDictionary(_parentValueContainer) is not { } dictionaryValue)
                return;
            var keyContainer = _environment.CurrentContext.GetVariable("key").ValueContainer;

            if (keyContainer.Value is not { } key)
            {
                _environment.ActiveException = RuntimeErrors.NullValueException;
                return;
            }

            var result = dictionaryValue[key];
            _environment.SaveResult(new ValueContainer(_environment.CurrentContext.ExpectedReturnType!, result));
            _environment.CurrentContext.IsReturning = true;
        }

        private void DictionaryAdd()
        {
            if (ValidateDictionary(_parentValueContainer) is not { } dictionaryValue)
                return;

            var keyContainer = _environment.CurrentContext.GetVariable("key").ValueContainer;
            var valueContainer = _environment.CurrentContext.GetVariable("value").ValueContainer;

            if (keyContainer.Value is not { } key)
            {
                _environment.ActiveException = RuntimeErrors.NullValueException;
                return;
            }

            try
            {
                dictionaryValue.Add(key, valueContainer.Value);
            }
            catch (ArgumentException)
            {
                _environment.ActiveException = RuntimeErrors.ItemAlreadyAddedException;
            }
        }

        private void DictionarySet()
        {
            if (ValidateDictionary(_parentValueContainer) is not { } dictionaryValue)
                return;

            var keyContainer = _environment.CurrentContext.GetVariable("key").ValueContainer;
            var valueContainer = _environment.CurrentContext.GetVariable("value").ValueContainer;

            if (keyContainer.Value is not { } key)
            {
                _environment.ActiveException = RuntimeErrors.NullValueException;
                return;
            }

            dictionaryValue[key] = valueContainer.Value;
        }

        private void DictionaryRemove()
        {
            if (ValidateDictionary(_parentValueContainer) is not { } dictionaryValue)
                return;

            var keyContainer = _environment.CurrentContext.GetVariable("key").ValueContainer;

            if (keyContainer.Value is not { } key)
            {
                _environment.ActiveException = RuntimeErrors.NullValueException;
                return;
            }

            dictionaryValue.Remove(key);
        }

        private IDictionary? ValidateDictionary(ValueContainer? container)
        {
            if (container is { Value: IDictionary dictionaryValue }) 
                return dictionaryValue;

            _environment.ActiveException = RuntimeErrors.NullValueException;
            return null;
        }
    }
}
