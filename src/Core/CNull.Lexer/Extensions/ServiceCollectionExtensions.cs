using CNull.Source.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CNull.Lexer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLexerServices(this IServiceCollection services)
        {
            services.AddSourceServices();
            services.AddSingleton<IRawLexer, Lexer>();
            services.AddSingleton<ILexer, CommentsFilterLexerProxy>();

            return services;
        }
    }
}
