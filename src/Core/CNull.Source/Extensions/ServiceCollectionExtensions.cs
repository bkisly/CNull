using Microsoft.Extensions.DependencyInjection;

namespace CNull.Source.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSourceServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IRawCodeSource, RawCodeSource>();
            serviceCollection.AddSingleton<ICodeSource, NewLineUnifierProxy>();
            return serviceCollection;
        }
    }
}
