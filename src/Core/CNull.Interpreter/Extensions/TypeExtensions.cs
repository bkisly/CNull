namespace CNull.Interpreter.Extensions
{
    public static class TypeExtensions
    {
        public static Type MakeNullableType(this Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            return type.IsValueType ? typeof(Nullable<>).MakeGenericType(type) : type;
        }
    }
}
