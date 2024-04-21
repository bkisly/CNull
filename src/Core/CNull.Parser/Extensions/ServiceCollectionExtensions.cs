using CNull.Lexer.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CNull.Parser.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers parser services.
        /// </summary>
        public static IServiceCollection AddParserServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddLexerServices();
            serviceCollection.AddSingleton<IParser, Parser>();
            return serviceCollection;
        }
    }
}
