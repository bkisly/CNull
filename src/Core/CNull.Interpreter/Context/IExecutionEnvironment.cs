using CNull.Interpreter.Symbols;

namespace CNull.Interpreter.Context
{
    public interface IExecutionEnvironment
    {
        CallContext CurrentContext { get; }
        string? ActiveException { get; set; }
        ValueContainer? ConsumeLastResult();
        void SaveResult(ValueContainer? result);
        void EnterCallContext(Type? returnType, IEnumerable<Variable> localVariables);
        void ExitCallContext();
    }
}
