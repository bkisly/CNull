using CNull.Interpreter.Symbols;

namespace CNull.Interpreter.Context
{
    public class InterpreterExecutionEnvironment : IExecutionEnvironment
    {
        private readonly Stack<CallContext> _contextsStack = [];

        public CallContext CurrentContext => _contextsStack.Peek();

        public IVariable? GetVariable(string name)
        {
            foreach (var scope in CurrentContext.Scopes)
            {
                if (scope.TryGetValue(name, out var variable))
                    return variable;
            }

            return null;
        }

        public void EnterCallContext(IEnumerable<IVariable> localVariables)
        {
            _contextsStack.Push(new CallContext(localVariables));
        }

        public void ExitCallContext()
        {
            _contextsStack.Pop();
        }
    }
}
