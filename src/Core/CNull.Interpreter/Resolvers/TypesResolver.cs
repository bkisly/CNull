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
                _ => throw new NotImplementedException("Cannot assign left to right...")
            };
        }

        public bool EnsureBoolean(object? value)
        {
            return value switch
            {
                bool boolValue => boolValue,
                null => throw new NotImplementedException("Used a nullable value in non-nullable context"),
                _ => throw new NotImplementedException("Expected a boolean type")
            };
        }

        public bool ResolveGreaterThan(object? left, object? right)
        {
            return (left, right) switch
            {
                (int leftInt, int rightInt) => leftInt > rightInt,
                (float, int) or (int, float) or (float, float) => Convert.ToSingle(left) > Convert.ToSingle(right),
                (string leftString, int rightInt) => leftString.Length > rightInt,
                (int leftInt, string rightString) => leftInt > rightString.Length,
                _ => throw new NotImplementedException("Cannot compare 2 values of types...")
            };
        }

        public bool ResolveEqualTo(object? left, object? right)
        {
            if (left == null || right == null)
                return left == right;

            bool? equal = (left, right) switch
            {
                (float, int) or (int, float) => Math.Abs(Convert.ToSingle(left) - Convert.ToSingle(right)) < float.Epsilon,
                (string leftString, int rightInt) => leftString.Length == rightInt,
                (int leftInt, string rightString) => leftInt == rightString.Length,
                _ => null
            };

            if (equal.HasValue)
                return equal.Value;

            if (left.GetType() == right.GetType())
                return left.Equals(right);

            throw new NotImplementedException("Cannot compare 2 values of types...");
        }

        public object ResolveAddition(object? left, object? right)
        {
            return (left, right) switch
            {
                (int leftInt, int rightInt) => leftInt + rightInt,
                (int, float) or (float, int) or (float, float) => Convert.ToSingle(left) + Convert.ToSingle(right),
                (null, _) or (_, null) => throw new NotImplementedException("Used null value in non nullable context"),
                _ => throw new NotImplementedException("Cannot add 2 values of types...")
            };
        }

        public object ResolveSubtraction(object? left, object? right)
        {
            return (left, right) switch
            {
                (int leftInt, int rightInt) => leftInt - rightInt,
                (int, float) or (float, int) or (float, float) => Convert.ToSingle(left) - Convert.ToSingle(right),
                (null, _) or (_, null) => throw new NotImplementedException("Used null value in non nullable context"),
                _ => throw new NotImplementedException("Cannot subtract 2 values of types...")
            };
        }

        public object ResolveMultiplication(object? left, object? right)
        {
            return (left, right) switch
            {
                (int leftInt, int rightInt) => leftInt * rightInt,
                (int, float) or (float, int) or (float, float) => Convert.ToSingle(left) * Convert.ToSingle(right),
                (null, _) or (_, null) => throw new NotImplementedException("Used null value in non nullable context"),
                _ => throw new NotImplementedException("Cannot multiply 2 values of types...")
            };
        }

        public object ResolveDivision(object? left, object? right)
        {
            return (left, right) switch
            {
                (int, 0) => throw new NotImplementedException("Division by zero"),
                (int leftInt, int rightInt) => leftInt / rightInt,
                (int, float) or (float, int) or (float, float) => Convert.ToSingle(left) / Convert.ToSingle(right),
                (null, _) or (_, null) => throw new NotImplementedException("Used null value in non nullable context"),
                _ => throw new NotImplementedException("Cannot divide 2 values of types...")
            };
        }

        public object ResolveModulo(object? left, object? right)
        {
            return (left, right) switch
            {
                (int, 0) => throw new NotImplementedException("Division by zero"),
                (int leftInt, int rightInt) => leftInt % rightInt,
                (int, float) or (float, int) or (float, float) => Convert.ToSingle(left) % Convert.ToSingle(right),
                (null, _) or (_, null) => throw new NotImplementedException("Used null value in non nullable context"),
                _ => throw new NotImplementedException("Cannot calculate modulo between 2 values of types...")
            };
        }

        public object ResolveNegation(object? value)
        {
            return value switch
            {
                int intValue => -intValue,
                float floatValue => -floatValue,
                null => throw new NotImplementedException("Used null value in non nullable context"),
                _ => throw new NotImplementedException("Cannot negate a value of type...")
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
