using CNull.Common;

namespace CNull.Interpreter.Context
{
    public class CallContext
    {
        public bool IsReturning { get; set; }
        public bool IsContinuing { get; set; }
        public bool IsBreaking { get; set; }

        public CallStackRecord CallStackRecord { get; set; }

        public bool IsJumping => IsReturning || IsContinuing || IsBreaking;

        public Type? ExpectedReturnType { get; private set; }
        public int LoopCounter { get; private set; }

        public event EventHandler? StackOverflowOccurred;

        private const int MaxLocalVariables = 100;
        private readonly Stack<Scope> _scopes = [];

        public CallContext(Type? returnType, IEnumerable<Variable> localVariables, CallStackRecord callStackRecord)
        {
            ExpectedReturnType = returnType;
            CallStackRecord = callStackRecord;

            EnterScope();
            var currentScope = _scopes.Peek();
            foreach (var localVariable in localVariables)
                currentScope.DeclareVariable(localVariable.Name, localVariable);
        }

        public Variable GetVariable(string name)
            => TryGetVariable(name)
               ?? throw new InvalidOperationException($"No variable named {name} is declared in the current scope.");

        public Variable? TryGetVariable(string name)
        {
            foreach (var scope in _scopes)
            {
                if (scope.TryGetValue(name, out var variable))
                    return variable;
            }

            return null;
        }

        public void DeclareVariable(Variable variable)
        {
            _scopes.Peek().DeclareVariable(variable.Name, variable);
        }

        public void EnterScope()
        {
            if (_scopes.Count == MaxLocalVariables)
                OnStackOverflowOccurred(this, EventArgs.Empty);
            else _scopes.Push(new Scope());
        }

        public void EnterLoopScope()
        {
            if (_scopes.Count == MaxLocalVariables)
            {
                OnStackOverflowOccurred(this, EventArgs.Empty);
                return;
            }

            _scopes.Push(new Scope());
            LoopCounter++;
        }

        public void ExitLoopScope()
        {
            _scopes.Pop();
            LoopCounter--;
        }

        public void ExitScope()
        {
            _scopes.Pop();
        }

        private void OnStackOverflowOccurred(object sender, EventArgs e) => StackOverflowOccurred?.Invoke(sender, e);
    }
}
