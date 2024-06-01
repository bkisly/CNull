using CNull.Interpreter.Symbols;

namespace CNull.Interpreter.Context
{
    public class Scope
    {
        private readonly Dictionary<string, IVariable> _localVariables = [];

        public bool TryGetValue(string key, out IVariable? value) => _localVariables.TryGetValue(key, out value);

        public void DeclareVariable(string name, IVariable variable)
        {
            _localVariables.Add(name, variable);
        }
    }
}
