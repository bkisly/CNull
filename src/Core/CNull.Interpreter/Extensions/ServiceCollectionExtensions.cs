using CNull.Semantics.Extensions;
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
            services.AddSemanticsServices();
            services.AddSingleton<IInterpreter, Interpreter>();
            return services;
        }
    }
}
