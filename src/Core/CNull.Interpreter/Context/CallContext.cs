using CNull.Interpreter.Symbols;

namespace CNull.Interpreter.Context
{
    public class CallContext
    {
        public bool IsReturning { get; set; }
        public Type? ExpectedReturnType { get; private set; }
        public int LoopCounter { get; private set; }

        private readonly Stack<Scope> _scopes = [];

        public CallContext(Type? returnType, IEnumerable<IVariable> localVariables)
        {
            ExpectedReturnType = returnType;
            EnterScope();
            var currentScope = _scopes.Peek();
            foreach (var localVariable in localVariables)
                currentScope.DeclareVariable(localVariable.Name, localVariable);
        }

        public IVariable? GetVariable(string name)
        {
            foreach (var scope in _scopes)
            {
                if (scope.TryGetValue(name, out var variable))
                    return variable;
            }

            return null;
        }

        public void DeclareVariable(IVariable variable)
        {
            _scopes.Peek().DeclareVariable(variable.Name, variable);
        }

        public void EnterScope()
        {
            _scopes.Push(new Scope());
        }

        public void EnterLoopScope()
        {
            _scopes.Push(new Scope());
            LoopCounter++;
        }
    }
}
