using CNull.Common.Extensions;
using CNull.Common.Mediators;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Events.Args;
using CNull.ErrorHandler.Exceptions;
using CNull.ErrorHandler.Extensions;
using CNull.Interpreter.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NReco.Logging.File;

namespace CNull.Interpreter
{
    /// <summary>
    /// Facade for the C? core library.
    /// </summary>
    public class CNullCore : IDisposable
    {
        private readonly IHost _host;
        private readonly IServiceScope _serviceScope;

        private readonly Func<string, string?> _inputCallback;
        private readonly Action<string> _outputCallback;
        private readonly Action<string> _errorCallback;

        private readonly ICoreComponentsMediator _mediator;
        private readonly IErrorHandler _errorHandler;
        private readonly IInterpreter _interpreter;

        /// <summary>
        /// <inheritdoc cref="CNullCore"/>
        /// </summary>
        /// <param name="inputCallback">Standard input provider. Parameter passes the string to display before requesting for input.</param>
        /// <param name="outputCallback">Standard output provider.</param>
        /// <param name="errorCallback">Error output provider.</param>
        public CNullCore(Func<string, string?> inputCallback, Action<string> outputCallback, Action<string> errorCallback)
        {
            _inputCallback = inputCallback;
            _outputCallback = outputCallback;
            _errorCallback = errorCallback;

            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddInterpreterServices()
                .AddErrorHandler()
                .AddCommonServices();

            builder.Logging.ClearProviders();
            builder.Logging.AddFile($"{DateTime.Now:yyyy-M-d-hh-mm-ss}.log", false);

            _host = builder.Build();
            _serviceScope = _host.Services.CreateScope();

            _mediator = _serviceScope.ServiceProvider.GetRequiredService<ICoreComponentsMediator>();
            _errorHandler = _serviceScope.ServiceProvider.GetRequiredService<IErrorHandler>();
            _interpreter = _serviceScope.ServiceProvider.GetRequiredService<IInterpreter>();

            _errorHandler.ErrorOccurred += ErrorHandler_ErrorOccurred;
        }

        private void ErrorHandler_ErrorOccurred(object? sender, ErrorOccurredEventArgs e)
        {
            _errorCallback.Invoke(e.Message);
        }

        /// <summary>
        /// Executes the program from the given file.
        /// </summary>
        /// <param name="path">Path to read the program from.</param>
        public async Task ExecuteFromFileAsync(string path) => await BeginExecutionAsync(() =>
        {
            _mediator.NotifyFileInputRequested(path);
            _interpreter.Execute(_inputCallback, _outputCallback);
        });

        private Task BeginExecutionAsync(Action executionAction)
        {
            try
            {
                executionAction.Invoke();
            }
            catch (FatalErrorException ex)
            {
                _errorCallback.Invoke(ex.Message);
            }
            catch (Exception ex)
            {
                _errorCallback.Invoke($"Unexpected error occurred. Error message: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _errorHandler.ErrorOccurred -= ErrorHandler_ErrorOccurred;
            _serviceScope.Dispose();
            _host.Dispose();
        }
    }
}
