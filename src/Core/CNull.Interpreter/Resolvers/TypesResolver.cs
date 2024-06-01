using CNull.ErrorHandler;
using CNull.Parser;
using CNull.Parser.Productions;

namespace CNull.Interpreter.Resolvers
{
    public class TypesResolver(IErrorHandler errorHandler)
    {
        public object? ResolveAssignment(object? left, object? right)
        {
            if (left?.GetType() == right?.GetType())
                return right;

            return (left, right) switch
            {
                (string, _) => right?.ToString(),
                (int, float or null) => (int?)right,
                (float, int or null) => right,
                (not null, null) => null,
                _ => throw new NotImplementedException()
            };
        }

        public Type ResolveDeclarableType(IDeclarableType type)
        {
            return type.Type switch
            {
                Types.Dictionary => ResolveDictionaryType((DictionaryType)type),
                _ => ResolvePrimitiveType(type)
            };
        }

        public Type? ResolveReturnType(ReturnType type)
        {
            return type is IDeclarableType declarableType ? ResolveDeclarableType(declarableType) : null;
        }

        private Type ResolveDictionaryType(DictionaryType type)
        {
            if (!type.KeyType.IsPrimitive || !type.ValueType.IsPrimitive)
                throw new NotImplementedException();

            var keyType = ResolvePrimitiveType(type.KeyType);
            var valueType = ResolvePrimitiveType(type.ValueType);

            return typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        }

        private Type ResolvePrimitiveType(IDeclarableType type)
        {
            if (!type.IsPrimitive)
                throw new NotImplementedException();

            return type.Type switch
            {
                Types.Boolean => typeof(bool),
                Types.Char => typeof(char),
                Types.Float => typeof(float),
                Types.Integer => typeof(int),
                Types.String => typeof(string),
                _ => throw new NotImplementedException()
            };
        }
    }
}
