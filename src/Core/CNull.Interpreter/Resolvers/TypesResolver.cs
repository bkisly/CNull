using CNull.ErrorHandler;
using CNull.Interpreter.Context;
using CNull.Interpreter.Errors;
using CNull.Parser;
using CNull.Parser.Productions;
using CNull.Semantics.Errors;

namespace CNull.Interpreter.Resolvers
{
    public delegate object BinaryOperationResolver(object? left, object? right, int lineNumber);

    public delegate bool BooleanBinaryOperationResolver(object? left, object? right, int lineNumber);

    public class TypesResolver(IExecutionEnvironment environment, IErrorHandler errorHandler)
    {
        public object? ResolveAssignment(ValueContainer left, ValueContainer right, int lineNumber)
        {
            return left.Type == right.Type ? right.Value : ResolveAssignmentValue(left.Value, right.Value, lineNumber);
        }

        public object? ResolveAssignment(object? left, object? right, int lineNumber)
        {
            return left?.GetType() == right?.GetType() ? right : ResolveAssignmentValue(left, right, lineNumber);
        }

        private object? ResolveAssignmentValue(object? left, object? right, int lineNumber)
        {
            return (left, right) switch
            {
                (string, _) => right?.ToString(),
                (int, float rightFloat) => (int)rightFloat,
                (float, int) => right,
                (not null, null) => null,
                _ => throw errorHandler.RaiseSemanticError(new TypeError(GetTypeName(left), GetTypeName(right), environment.CurrentModule, lineNumber))
            };
        }

        private static string GetTypeName(object? value) => value?.GetType().Name ?? "null";

        public bool EnsureBoolean(object? value, int lineNumber)
        {
            return value switch
            {
                bool boolValue => boolValue,
                null => InvalidNullUsage<bool>(lineNumber),
                _ => throw errorHandler.RaiseSemanticError(new TypeError(nameof(Boolean), GetTypeName(value), environment.CurrentModule, lineNumber))
            };
        }

        public int ResolveRelational(object? left, object? right, int lineNumber)
        {
            return (left, right) switch
            {
                (int leftInt, int rightInt) => leftInt.CompareTo(rightInt),
                (float, int) or (int, float) or (float, float) => Convert.ToSingle(left).CompareTo(Convert.ToSingle(right)),
                (string leftString, int rightInt) => leftString.Length.CompareTo(rightInt),
                (int leftInt, string rightString) => leftInt.CompareTo(rightString.Length),
                (null, _) or (_, null) => InvalidNullUsage<int>(lineNumber),
                _ => throw errorHandler.RaiseSemanticError(new TypeError("relational expression", GetTypeName(left),
                    GetTypeName(right), environment.CurrentModule, lineNumber))
            };
        }

        public bool ResolveEquality(object? left, object? right, int lineNumber)
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

            throw errorHandler.RaiseSemanticError(new TypeError("equality expression", GetTypeName(left),
                GetTypeName(right), environment.CurrentModule, lineNumber));
        }

        public object ResolveAddition(object? left, object? right, int lineNumber)
        {
            return (left, right) switch
            {
                (int leftInt, int rightInt) => leftInt + rightInt,
                (int, float) or (float, int) or (float, float) => Convert.ToSingle(left) + Convert.ToSingle(right),
                (null, _) or (_, null) => InvalidNullUsage<int>(lineNumber),
                (string leftString, string rightString) => leftString + rightString,
                (char leftChar, char rightChar) => leftChar + rightChar,
                (string leftString, not null) => leftString + right,
                (not null, string rightString) => left + rightString,
                _ => throw errorHandler.RaiseSemanticError(new TypeError("+", GetTypeName(left), GetTypeName(right), environment.CurrentModule, lineNumber))
            };
        }

        public object ResolveSubtraction(object? left, object? right, int lineNumber)
        {
            if ((left, right) is (int leftInt, int rightInt))
                return leftInt - rightInt;

            return (left, right) switch
            {
                (int, float) or (float, int) or (float, float) => Convert.ToSingle(left) - Convert.ToSingle(right),
                (null, _) or (_, null) => InvalidNullUsage<int>(lineNumber),
                _ => throw errorHandler.RaiseSemanticError(new TypeError("-", GetTypeName(left), GetTypeName(right), environment.CurrentModule, lineNumber))
            };
        }

        public object ResolveMultiplication(object? left, object? right, int lineNumber)
        {
            if ((left, right) is (int leftInt, int rightInt))
                return leftInt * rightInt;

            return (left, right) switch
            {
                (int, float) or (float, int) or (float, float) => Convert.ToSingle(left) * Convert.ToSingle(right),
                (null, _) or (_, null) => InvalidNullUsage<int>(lineNumber),
                _ => throw errorHandler.RaiseSemanticError(new TypeError("*", GetTypeName(left), GetTypeName(right), environment.CurrentModule, lineNumber))
            };
        }

        public object ResolveDivision(object? left, object? right, int lineNumber)
        {
            if ((left, right) is (int leftInt, int rightInt))
            {
                if (rightInt != 0) 
                    return leftInt / rightInt;

                environment.ActiveException = new ExceptionInfo(RuntimeErrors.DivisionByZeroException, lineNumber);
                return 0;
            }

            return (left, right) switch
            {
                (int, float) or (float, int) or (float, float) => Convert.ToSingle(left) / Convert.ToSingle(right),
                (null, _) or (_, null) => InvalidNullUsage<int>(lineNumber),
                _ => throw errorHandler.RaiseSemanticError(new TypeError("/", GetTypeName(left), GetTypeName(right), environment.CurrentModule, lineNumber))
            };
        }

        public object ResolveModulo(object? left, object? right, int lineNumber)
        {
            if ((left, right) is (int leftInt, int rightInt))
            {
                if (rightInt != 0)
                    return leftInt / rightInt;

                environment.ActiveException = new ExceptionInfo(RuntimeErrors.DivisionByZeroException, lineNumber);
                return 0;
            }

            return (left, right) switch
            {
                (int, float) or (float, int) or (float, float) => Convert.ToSingle(left) % Convert.ToSingle(right),
                (null, _) or (_, null) => InvalidNullUsage<int>(lineNumber),
                _ => throw errorHandler.RaiseSemanticError(new TypeError("%", GetTypeName(left), GetTypeName(right), environment.CurrentModule, lineNumber))
            };
        }

        public object ResolveNegation(object? value, int lineNumber)
        {
            if (value is int intValue)
                return -intValue;

            return value switch
            {
                float floatValue => -floatValue,
                null => InvalidNullUsage<int>(lineNumber),
                _ => throw errorHandler.RaiseSemanticError(new TypeError($"{nameof(Int32)} or {nameof(Single)}", GetTypeName(value), environment.CurrentModule, lineNumber))
            };
        }

        public static Type ResolveDeclarableType(IDeclarableType type)
        {
            return type.Type switch
            {
                Types.Dictionary => ResolveDictionaryType((DictionaryType)type),
                _ => ResolvePrimitiveType((PrimitiveType)type)
            };
        }

        public static Type? ResolveReturnType(ReturnType type)
        {
            return type is IDeclarableType declarableType ? ResolveDeclarableType(declarableType) : null;
        }

        private static Type ResolveDictionaryType(DictionaryType type)
        {
            var keyType = ResolvePrimitiveType(type.KeyType);
            var valueType = ResolvePrimitiveType(type.ValueType);

            return typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        }

        private static Type ResolvePrimitiveType(PrimitiveType type)
        {
            return ResolvePrimitiveType(type.TypeSpecifier) ?? throw new ArgumentOutOfRangeException();
        }

        public static Type? ResolvePrimitiveType(PrimitiveTypes type)
        {
            return type switch
            {
                PrimitiveTypes.Boolean => typeof(bool?),
                PrimitiveTypes.Char => typeof(char?),
                PrimitiveTypes.Float => typeof(float?),
                PrimitiveTypes.Integer => typeof(int?),
                PrimitiveTypes.String => typeof(string),
                _ => null
            };
        }

        private T InvalidNullUsage<T>(int lineNumber) where T : struct
        {
            environment.ActiveException = new ExceptionInfo(RuntimeErrors.NullValueException, lineNumber);
            return default;
        }
    }
}
