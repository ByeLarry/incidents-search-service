using SearchService.Options;
using SearchService.Interfaces;
using SearchService.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace SearchService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks();
            return services;
        }

        public static IServiceCollection AddMessageHandlersService(this IServiceCollection services)
        {
            services.AddSingleton<MessageHandlersService>();
            return services;
        }

        public static IServiceCollection AddRabbitMQServices(this IServiceCollection services, IConfiguration configuration) 
        {
            services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQOptions"));
            services.AddSingleton<IRabbitMQService, RabbitMQService>();
            return services;
        }

        public static IServiceCollection AddElasticsearchServices(this IServiceCollection services, IConfiguration configuration)
        {
            var elasticsearchSettings = configuration.GetSection("Elasticsearch").Get<ElasticsearchOptoins>();

            if (elasticsearchSettings != null)
            {
                services.AddSingleton(provider =>
                {
                    var settings = new ElasticsearchClientSettings(new Uri(elasticsearchSettings.Url))
                        .Authentication(new BasicAuthentication(elasticsearchSettings.Username, elasticsearchSettings.Password));

                    return new ElasticsearchClient(settings);
                });
            }

            services.AddSingleton<ElasticSearchService>();
            return services;
        }
    }
}
