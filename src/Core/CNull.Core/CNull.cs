using CNull.Common.Extensions;
using CNull.ErrorHandler.Extensions;
using CNull.Interpreter.Extensions;
using Microsoft.Extensions.Hosting;

namespace CNull.Core
{
    /// <summary>
    /// Facade for the C? core library.
    /// </summary>
    public class CNull : IDisposable
    {
        private readonly IHost _host;

        public CNull()
        {
            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddInterpreterServices();
            builder.Services.AddErrorHandler();
            builder.Services.AddCommonServices();

            _host = builder.Build();
        }

        public void Dispose()
        {
            _host.Dispose();
        }
    }
}
