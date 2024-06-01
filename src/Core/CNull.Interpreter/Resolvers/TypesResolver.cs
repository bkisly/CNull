using CNull.Parser;
using CNull.Parser.Productions;

namespace CNull.Interpreter.Resolvers
{
    public static class TypesResolver
    {
        public static object? ResolveAssignment(object? left, object? right)
        {
            if (left?.GetType() == right?.GetType())
                return right;

            return (left, right) switch
            {
                (string or null, _) => right?.ToString(),
                (int or null, float or null) => (int?)right,
                (float or null, int or null) => right,
                _ => throw new NotImplementedException()
            };
        }

        public static object? ResolveType(IDeclarableType type)
        {
            return type.Type switch
            {
                Types.Dictionary => ResolveDictionaryType((DictionaryType)type),
                _ => ResolvePrimitiveType(type)
            };
        }

        private static object? ResolveDictionaryType(DictionaryType type)
        {
            if (!type.KeyType.IsPrimitive || !type.ValueType.IsPrimitive)
                return null!;

            var keyType = ResolvePrimitiveType(type.KeyType);
            var valueType = ResolvePrimitiveType(type.ValueType);

            var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType.GetType(), valueType.GetType());
            return Activator.CreateInstance(dictionaryType);
        }

        private static object ResolvePrimitiveType(IDeclarableType type)
        {
            if (!type.IsPrimitive)
                throw new NotImplementedException();

            return type.Type switch
            {
                Types.Boolean => default(bool),
                Types.Char => default(char),
                Types.Float => default(float),
                Types.Integer => default(int),
                Types.String => string.Empty,
                _ => throw new NotImplementedException()
            };
        }
    }
}
