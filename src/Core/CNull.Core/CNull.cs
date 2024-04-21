using CNull.Common.Extensions;
using CNull.Common.Mediators;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Events.Args;
using CNull.ErrorHandler.Extensions;
using CNull.Interpreter;
using CNull.Interpreter.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CNull.Core
{
    /// <summary>
    /// Facade for the C? core library.
    /// </summary>
    public class CNull : IDisposable
    {
        private readonly IHost _host;
        private readonly IServiceScope _serviceScope;

        private readonly Func<string, string> _inputCallback;
        private readonly Action<string> _outputCallback;
        private readonly Action<string> _errorCallback;

        private ICoreComponentsMediator Mediator => _serviceScope.ServiceProvider.GetRequiredService<ICoreComponentsMediator>();
        private IErrorHandler ErrorHandler => _serviceScope.ServiceProvider.GetRequiredService<IErrorHandler>();
        private IInterpreter Interpreter => _serviceScope.ServiceProvider.GetRequiredService<IInterpreter>();

        public CNull(Func<string, string> inputCallback, Action<string> outputCallback, Action<string> errorCallback)
        {
            _inputCallback = inputCallback;
            _outputCallback = outputCallback;
            _errorCallback = errorCallback;

            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddInterpreterServices();
            builder.Services.AddErrorHandler();
            builder.Services.AddCommonServices();

            _host = builder.Build();
            _serviceScope = _host.Services.CreateScope();

            ErrorHandler.ErrorOccurred += ErrorHandler_ErrorOccurred;
        }

        public void ExecuteFromFile(string path)
        {
            Mediator.NotifyFileInputRequested(path);
            Interpreter.Execute(_inputCallback, _outputCallback);
        }

        public void Dispose()
        {
            ErrorHandler.ErrorOccurred -= ErrorHandler_ErrorOccurred;
            _serviceScope.Dispose();
            _host.Dispose();
        }

        private void ErrorHandler_ErrorOccurred(object? sender, ErrorOccurredEventArgs e)
        {
            _errorCallback.Invoke(e.Message);
        }
    }
}
