using CNull.Parser.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CNull.Semantics.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSemanticsServices(this IServiceCollection services)
        {
            services.AddParserServices();
            services.AddSingleton<ISemanticAnalyzer, SemanticAnalyzer>();
            return services;
        }
    }
}
