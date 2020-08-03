using System.Linq;
using System.Reflection;
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

        public static IServiceCollection AddConfiguration<TConfiguration>(this IServiceCollection services, string sectionName)
            where TConfiguration : class, new()
        {
            return services.AddTransient<TConfiguration>((serviceProvider) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                
                var configDetails = new TConfiguration(); 
                foreach (var propertyInfo in configDetails.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var settingName = string.Concat(sectionName, ":", propertyInfo.Name);
                    var settingValue = config.GetValue(propertyInfo.PropertyType, settingName);
                    if(settingValue != null)
                        propertyInfo.SetValue(configDetails, settingValue);
                }
                return configDetails;
            });
        }
    }
}
