using CNull.Interpreter.Symbols;

namespace CNull.Interpreter.Context
{
    public interface IExecutionEnvironment
    {
        CallContext CurrentContext { get; }
        IVariable? GetVariable(string name);
        void EnterCallContext(IEnumerable<IVariable> localVariables);
        void ExitCallContext();
    }
}
