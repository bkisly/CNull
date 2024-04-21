using Microsoft.Extensions.DependencyInjection;

namespace CNull.ErrorHandler.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddErrorHandler(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IErrorHandler, ErrorHandler>();
            return serviceCollection;
        }
    }
}
