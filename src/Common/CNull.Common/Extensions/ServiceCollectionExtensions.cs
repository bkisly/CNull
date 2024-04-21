using CNull.Common.Mediators;
using Microsoft.Extensions.DependencyInjection;

namespace CNull.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers services that are used across the whole application.
        /// </summary>
        public static IServiceCollection AddCommonServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICoreComponentsMediator, CoreComponentsMediator>();
            return serviceCollection;
        }
    }
}
