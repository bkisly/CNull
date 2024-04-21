using CNull.Parser.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CNull.Interpreter.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers interpreter services.
        /// </summary>
        public static IServiceCollection AddInterpreterServices(this IServiceCollection services)
        {
            services.AddParserServices();
            services.AddSingleton<IInterpreter, Interpreter>();
            return services;
        }
    }
}
