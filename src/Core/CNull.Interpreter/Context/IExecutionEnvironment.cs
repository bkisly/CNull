using CNull.Interpreter.Symbols;

namespace CNull.Interpreter.Context
{
    public interface IExecutionEnvironment
    {
        CallContext CurrentContext { get; }
        object? ConsumeLastResult();
        void SaveResult(object? result);
        void EnterCallContext(Type? returnType, IEnumerable<IVariable> localVariables);
        void ExitCallContext();
    }
}
