namespace CNull.Interpreter.Context
{
    public record Variable(string Name, ValueContainer ValueContainer)
    {
        public ValueContainer ValueContainer { get; set; } = ValueContainer;
    }

    public record ValueContainer(Type Type, object? Value, bool IsPrimitive = true)
    {
        public Type Type { get; set; } = Type;
        public object? Value { get; set; } = Value;

        public ValueContainer Move()
        {
            return IsPrimitive ? new ValueContainer(Type, Value, IsPrimitive) : this;
        }
    }
}
