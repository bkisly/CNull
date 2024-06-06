using CNull.Common.Extensions;
using CNull.Common.State;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Events;
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

        private readonly StandardInput _inputCallback;
        private readonly StandardOutput _outputCallback;
        private readonly StandardError _errorCallback;

        private readonly string[] _args;

        private readonly IStateManager _stateManager;
        private readonly IErrorHandler _errorHandler;
        private readonly IInterpreter _interpreter;

        /// <summary>
        /// <inheritdoc cref="CNullCore"/>
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <param name="inputCallback">Standard input callback.</param>
        /// <param name="outputCallback">Standard output callback.</param>
        /// <param name="errorCallback">Error output callback.</param>
        public CNullCore(string[] args, StandardInput inputCallback, StandardOutput outputCallback, StandardError errorCallback)
        {
            _args = args;
            _inputCallback = inputCallback;
            _outputCallback = outputCallback;
            _errorCallback = errorCallback;

            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddInterpreterServices()
                .AddErrorHandler()
                .AddCommonServices();

            builder.Logging.ClearProviders();
            builder.Logging.AddFile(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CNull/Logs", $"{DateTime.Now:yyyy-M-d-hh-mm-ss}.log"),
                false);

            _host = builder.Build();
            _serviceScope = _host.Services.CreateScope();
            var serviceProvider = _serviceScope.ServiceProvider;

            _stateManager = serviceProvider.GetRequiredService<IStateManager>();
            _errorHandler = serviceProvider.GetRequiredService<IErrorHandler>();
            _interpreter = serviceProvider.GetRequiredService<IInterpreter>();

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
            _stateManager.NotifyInputRequested(GetRootFilePath(path));
        });

        /// <summary>
        /// Executes the program from the given stream. This method does not support multi-moduled programs.
        /// </summary>
        /// <param name="stream">Stream to read the program from.</param>
        public async Task ExecuteFromStreamAsync(Lazy<Stream> stream) => await BeginExecutionAsync(() =>
        {
            _stateManager.NotifyInputRequested(stream);
        });

        private Task BeginExecutionAsync(Action executionAction)
        {
            try
            {
                executionAction.Invoke();
                _interpreter.Execute(_args, _inputCallback, _outputCallback);
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

        private static string GetRootFilePath(string path)
        {
            return Directory.Exists(path) ? Path.Combine(path, "Program.cnull") : path;
        }
    }
}
