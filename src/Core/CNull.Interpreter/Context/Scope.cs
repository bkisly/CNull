namespace CNull.Interpreter.Context
{
    public class Scope
    {
        private readonly Dictionary<string, Variable> _localVariables = [];

        public bool TryGetValue(string key, out Variable? value) => _localVariables.TryGetValue(key, out value);

        public void DeclareVariable(string name, Variable variable)
        {
            _localVariables.Add(name, variable);
        }
    }
}
