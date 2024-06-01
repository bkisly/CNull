namespace CNull.Interpreter.Symbols
{
    public interface IVariable
    {
        Type Type { get; }
        string Name { get; }
        object? Value { get; set; }
    }

    public record struct ValueTypeVariable(Type Type, string Name, object? Value) : IVariable
    {
        public Type Type { get; } = Type;
        public string Name { get; } = Name;
    }

    public record ReferenceTypeVariable(Type Type, string Name, object? Value) : IVariable
    {
        public object? Value { get; set; } = Value;
    }
}
