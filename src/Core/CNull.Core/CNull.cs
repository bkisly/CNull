using CNull.Common.Extensions;
using CNull.Common.Mediators;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Events.Args;
using CNull.ErrorHandler.Exceptions;
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

        private readonly Func<string, string?> _inputCallback;
        private readonly Action<string> _outputCallback;
        private readonly Action<string> _errorCallback;

        private readonly ICoreComponentsMediator _mediator;
        private readonly IErrorHandler _errorHandler;
        private readonly IInterpreter _interpreter;

        public CNull(Func<string, string?> inputCallback, Action<string> outputCallback, Action<string> errorCallback)
        {
            _inputCallback = inputCallback;
            _outputCallback = outputCallback;
            _errorCallback = errorCallback;

            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddInterpreterServices()
                .AddErrorHandler()
                .AddCommonServices();

            _host = builder.Build();
            _serviceScope = _host.Services.CreateScope();

            _mediator = _serviceScope.ServiceProvider.GetRequiredService<ICoreComponentsMediator>();
            _errorHandler = _serviceScope.ServiceProvider.GetRequiredService<IErrorHandler>();
            _interpreter = _serviceScope.ServiceProvider.GetRequiredService<IInterpreter>();

            _errorHandler.ErrorOccurred += ErrorHandler_ErrorOccurred;
        }

        public async Task ExecuteFromFileAsync(string path) => await BeginExecutionAsync(() =>
        {
            _mediator.NotifyFileInputRequested(path);
            _interpreter.Execute(_inputCallback, _outputCallback);
        });

        public void Dispose()
        {
            _errorHandler.ErrorOccurred -= ErrorHandler_ErrorOccurred;
            _serviceScope.Dispose();
            _host.Dispose();
        }

        private Task BeginExecutionAsync(Action executionAction)
        {
            try
            {
                executionAction.Invoke();
            }
            catch (FatalErrorException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Task.CompletedTask;
        }

        private void ErrorHandler_ErrorOccurred(object? sender, ErrorOccurredEventArgs e)
        {
            _errorCallback.Invoke(e.Message);
        }
    }
}
