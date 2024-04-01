using CNull.ErrorHandler.Errors;

namespace CNull.ErrorHandler
{
    public class ErrorHandler : IErrorHandler
    {
        public void RaiseSourceError(ISourceError error)
        {
            throw new NotImplementedException();
        }

        public void RaiseCompilationError(ICompilationError error)
        {
            throw new NotImplementedException();
        }

        public void RaiseRuntimeError(IRuntimeError error)
        {
            throw new NotImplementedException();
        }
    }
}
