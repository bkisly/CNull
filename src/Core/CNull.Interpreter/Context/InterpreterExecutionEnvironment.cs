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

        private const int MaxCallStack = 800;
        public event EventHandler? StackOverflowOccurred;

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
                        .Union(_contextsStack.Select(c => c.CallStackRecord))
                        .ToArray();
                _activeException = value;
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
            if (_contextsStack.Count == MaxCallStack)
                OnStackOverflowOccurred(this, EventArgs.Empty);
            else
            {
                var context = new CallContext(returnType, localVariables, callStackRecord);
                context.StackOverflowOccurred += OnStackOverflowOccurred;
                _contextsStack.Push(context);
            }
        }

        public void ExitCallContext()
        {
            var context = _contextsStack.Pop();
            context.StackOverflowOccurred -= OnStackOverflowOccurred;
        }

        public IEnumerable<CallStackRecord> GetRecentCallStackRecords(int number)
        {
            return _contextsStack.Select(c => c.CallStackRecord).Take(number);
        }

        private void OnStackOverflowOccurred(object? sender, EventArgs e) => StackOverflowOccurred?.Invoke(sender, e);
    }
}
