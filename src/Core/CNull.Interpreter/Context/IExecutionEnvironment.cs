using CNull.Interpreter.Symbols;

namespace CNull.Interpreter.Context
{
    public interface IExecutionEnvironment
    {
        CallContext CurrentContext { get; }
        IValueContainer? ConsumeLastResult();
        void SaveResult(IValueContainer? result);
        void EnterCallContext(Type? returnType, IEnumerable<Variable> localVariables);
        void ExitCallContext();
    }
}
