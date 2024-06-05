using CNull.Common;
using CNull.Interpreter.Symbols;

namespace CNull.Interpreter.Context
{
    public class InterpreterExecutionEnvironment : IExecutionEnvironment
    {
        private readonly Stack<CallContext> _contextsStack = [];
        private ValueContainer? _lastResult;

        public string CurrentModule { get; set; } = null!;
        public string CurrentFunction { get; set; } = "<entry point>";
        public CallContext CurrentContext => _contextsStack.Peek();

        private ExceptionInfo? _activeException;

        public ExceptionInfo? ActiveException
        {
            get => _activeException;
            set
            {
                if (value == null)
                {
                    _activeException = value;
                    return;
                }

                value.StackTrace = new[] { new CallStackRecord(CurrentModule, CurrentFunction, value.LineNumber) }
                        .Union(_contextsStack.Select(c => c.CallStackRecord));
            }
        }

        public ValueContainer ConsumeLastResult()
        {
            var result = _lastResult;
            _lastResult = null;
            return result ?? throw new NullReferenceException("Tried to consume already consumed value.");
        }

        public void SaveResult(ValueContainer? result)
        {
            _lastResult = result;
        }

        public void EnterCallContext(Type? returnType, IEnumerable<Variable> localVariables, CallStackRecord callStackRecord)
        {
            _contextsStack.Push(new CallContext(returnType, localVariables, callStackRecord));
        }

        public void ExitCallContext()
        {
            _contextsStack.Pop();
        }
    }
}
