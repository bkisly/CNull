using CNull.Interpreter.Symbols;

namespace CNull.Interpreter.Context
{
    public class CallContext
    {
        public bool IsReturning { get; private set; }
        public Type? ExpectedReturnType { get; private set; } = null!;
        public int LoopCounter { get; private set; }

        public object? LastResult { get; private set; }
        public Stack<Scope> Scopes { get; private set; } = [];

        public CallContext(IEnumerable<IVariable> localVariables)
        {
            EnterScope();
            var currentScope = Scopes.Peek();
            foreach (var localVariable in localVariables)
                currentScope.DeclareVariable(localVariable.Name, localVariable);
        }

        public void EnterScope()
        {
            Scopes.Push(new Scope());
        }

        public void EnterLoopScope()
        {
            Scopes.Push(new Scope());
            LoopCounter--;
        }
    }
}
