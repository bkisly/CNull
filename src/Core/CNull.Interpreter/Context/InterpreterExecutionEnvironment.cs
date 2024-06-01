using CNull.Interpreter.Symbols;

namespace CNull.Interpreter.Context
{
    public class InterpreterExecutionEnvironment : IExecutionEnvironment
    {
        private readonly Stack<CallContext> _contextsStack = [];
        private object? _lastResult;

        public CallContext CurrentContext => _contextsStack.Peek();

        public object? ConsumeLastResult()
        {
            var result = _lastResult;
            _lastResult = null;
            return result;
        }

        public void SaveResult(object? result)
        {
            _lastResult = result;
        }

        public void EnterCallContext(Type? returnType, IEnumerable<IVariable> localVariables)
        {
            _contextsStack.Push(new CallContext(returnType, localVariables));
        }

        public void ExitCallContext()
        {
            _contextsStack.Pop();
        }
    }
}
