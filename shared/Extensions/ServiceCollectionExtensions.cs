using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configuration;

namespace Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCosmosClient(this IServiceCollection services)
        {
            return services.AddSingleton((provider) =>
            {
                var cosmosDbConfig = provider.GetRequiredService<CosmosDbConfiguration>();
                return cosmosDbConfig.CreateCosmosClient();
            });
        }

        public static IServiceCollection AddConfigurationSection<TConfiguration>(this IServiceCollection services, string sectionName)
            where TConfiguration : class
        {
            return services.AddTransient<TConfiguration>((serviceProvider) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var section = config.GetSection(sectionName);
                return section.Get<TConfiguration>();
            });
        }
    }
}
