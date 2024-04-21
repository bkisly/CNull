using CNull.Source.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CNull.Source.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSourceServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IInputRepository, InputRepository>();
            serviceCollection.AddSingleton<IRawCodeSource, RawCodeSource>();
            serviceCollection.AddSingleton<ICodeSource, NewLineUnifierProxy>();
            return serviceCollection;
        }
    }
}
