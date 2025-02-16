using SearchService.Options;
using SearchService.Interfaces;
using SearchService.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace SearchService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
			var elasticsearchOptions = configuration.GetSection("ElasticsearchOptions").Get<ElasticSearchOptions>();

			if (elasticsearchOptions is null)
				throw new ArgumentNullException(nameof(elasticsearchOptions));

			services
				.AddHealthChecks()
                .AddDiskStorageHealthCheck(options =>
                 {
                     options.AddDrive("/", 1024);
                 }, name: "disk", tags: new[] { "health-checks" })
                .AddUrlGroup(new Uri("http://localhost:8080"), name: "self", failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy, tags: new[] { "search" })
                .AddProcessAllocatedMemoryHealthCheck(500 * 1024 * 1024, "memory_heap", tags: new[] { "search" }) // Порог 500MB
                .AddWorkingSetHealthCheck(1024 * 1024 * 1024, "memory_rss", tags: new[] { "search" })
				.AddElasticsearch(setup: options =>
				{
					options.UseServer(elasticsearchOptions.Uri);
					options.UseBasicAuthentication(elasticsearchOptions.Username, elasticsearchOptions.Password);

				}, name: "Elasticsearch", failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy, tags: new[] { "health-checks", "search" }); // Порог 1GB

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
            var elasticsearchSettings = configuration.GetSection("ElasticsearchOptions").Get<ElasticSearchOptions>();

            if (elasticsearchSettings != null)
            {
                services.AddSingleton(provider =>
                {
                    var settings = new ElasticsearchClientSettings(new Uri(elasticsearchSettings.Uri))
                        .Authentication(new BasicAuthentication(elasticsearchSettings.Username, elasticsearchSettings.Password));

                    return new ElasticsearchClient(settings);
                });
            }

            services.AddSingleton<ElasticSearchService>();
            return services;
        }
    }
}
