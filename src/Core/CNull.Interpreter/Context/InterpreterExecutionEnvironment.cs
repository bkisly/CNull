using CNull.Interpreter.Symbols;

namespace CNull.Interpreter.Context
{
    public class InterpreterExecutionEnvironment : IExecutionEnvironment
    {
        private readonly Stack<CallContext> _contextsStack = [];
        private ValueContainer? _lastResult;

        public CallContext CurrentContext => _contextsStack.Peek();
        public string? ActiveException { get; set; }

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

        public void EnterCallContext(Type? returnType, IEnumerable<Variable> localVariables)
        {
            _contextsStack.Push(new CallContext(returnType, localVariables));
        }

        public void ExitCallContext()
        {
            _contextsStack.Pop();
        }
    }
}
