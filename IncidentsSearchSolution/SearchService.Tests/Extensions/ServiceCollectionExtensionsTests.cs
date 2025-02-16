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
			// Arrange
			var services = new ServiceCollection();
			var inMemoryConfiguration = new Dictionary<string, string>
			{
				{ "ElasticsearchOptions:Uri", "http://host.docker.internal:9200" },
				{ "ElasticsearchOptions:Username", "elastic" },
				{ "ElasticsearchOptions:Password", "1111" }
			};

			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(inMemoryConfiguration)
				.Build();

			// Act
			services.AddElasticsearchServices(configuration);
			var serviceProvider = services.BuildServiceProvider();

			// Assert: проверяем регистрацию в DI-контейнере
			services.Should().ContainSingle(sd => sd.ServiceType == typeof(ElasticsearchClient));
			services.Should().ContainSingle(sd => sd.ServiceType == typeof(ElasticSearchService));

			// Assert: проверяем создание экземпляра
			var elasticClient = serviceProvider.GetService<ElasticsearchClient>();
			elasticClient.Should().NotBeNull();

			var elasticSearchService = serviceProvider.GetService<ElasticSearchService>();
			elasticSearchService.Should().NotBeNull();
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
