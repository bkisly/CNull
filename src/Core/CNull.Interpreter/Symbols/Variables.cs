namespace CNull.Interpreter.Symbols
{
    public interface IVariable
    {
        string Name { get; }
        object? Value { get; set; }
    }

    public record struct ValueTypeVariable(string Name, object? Value) : IVariable
    {
        public string Name { get; } = Name;
    }

    public record ReferenceTypeVariable(string Name, object? Value) : IVariable
    {
        public object? Value { get; set; } = Value;
    }
}
