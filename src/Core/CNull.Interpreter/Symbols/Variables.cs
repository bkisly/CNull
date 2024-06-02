namespace CNull.Interpreter.Symbols
{
    public class Variable(string name, IValueContainer valueContainer)
    {
        public string Name { get; } = name;
        public IValueContainer ValueContainer { get; } = valueContainer;
    }

    public interface IValueContainer
    {
        Type? Type { get; }
        object? Value { get; set; }
    }

    public record struct ValueTypeContainer(Type? Type, object? Value) : IValueContainer
    {
        public Type? Type { get; } = Type;
    }

    public record ReferenceTypeContainer(Type? Type, object? Value) : IValueContainer
    {
        public object? Value { get; set; } = Value;
    }
}
