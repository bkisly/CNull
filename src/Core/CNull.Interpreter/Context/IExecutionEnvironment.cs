using CNull.Common;
using CNull.Interpreter.Symbols;

namespace CNull.Interpreter.Context
{
    public record ExceptionInfo(string Exception, int LineNumber)
    {
        public IEnumerable<CallStackRecord> StackTrace { get; set; } = null!;
    }

    public interface IExecutionEnvironment
    {
        string CurrentModule { get; set; }
        string CurrentFunction { get; set; }
        CallContext CurrentContext { get; }
        ExceptionInfo? ActiveException { get; set; }
        ValueContainer? ConsumeLastResult();
        void SaveResult(ValueContainer? result);
        void EnterCallContext(Type? returnType, IEnumerable<Variable> localVariables, CallStackRecord callStackRecord);
        void ExitCallContext();
    }
}
