using FluentAssertions;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SearchService.Extensions;
using SearchService.Interfaces;
using SearchService.Options;
using SearchService.Services;
using Elastic.Clients.Elasticsearch;

namespace SearchService.Tests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {

        [Fact]
        public void AddMessageHandlersService_ShouldRegisterMessageHandlersServiceAsSingleton()
        {
            var services = new ServiceCollection();

            services.AddMessageHandlersService();

            services.Should().ContainSingle(sd => sd.ServiceType == typeof(MessageHandlersService) && sd.Lifetime == ServiceLifetime.Singleton);
        }

        [Fact]
        public void AddRabbitMQServices_ShouldRegisterRabbitMQServiceWithConfiguration()
        {
            var services = new ServiceCollection();
            var configuration = A.Fake<IConfiguration>();

            services.AddRabbitMQServices(configuration);

            services.Should().ContainSingle(sd => sd.ServiceType == typeof(IRabbitMQService) && sd.ImplementationType == typeof(RabbitMQService));
        }

        [Fact]
        public void AddElasticsearchServices_ShouldRegisterElasticsearchClientAndService_WhenConfigurationIsValid()
        {
            var services = new ServiceCollection();
            var inMemoryConfiguration = new Dictionary<string, string>
            {
                { "Elasticsearch:Url", "http://localhost:9200" },
                { "Elasticsearch:Username", "elastic" },
                { "Elasticsearch:Password", "password" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfiguration)
                .Build();

            services.AddElasticsearchServices(configuration);

            services.Should().ContainSingle(sd => sd.ServiceType == typeof(ElasticsearchClient));
            services.Should().ContainSingle(sd => sd.ServiceType == typeof(ElasticSearchService));
        }



        [Fact]
        public void AddElasticsearchServices_ShouldNotRegisterElasticsearchClient_WhenConfigurationIsNull()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build(); 

            services.AddElasticsearchServices(configuration);

            services.Should().NotContain(sd => sd.ServiceType == typeof(ElasticsearchClient));

            services.Should().ContainSingle(sd => sd.ServiceType == typeof(ElasticSearchService));
        }

    }
}
