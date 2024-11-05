
using api.Interfaces;
using api.Services;
using api.Options;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQOptions"));
        builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();

        builder.Services.Configure<ElasticsearchOptoins>(builder.Configuration.GetSection("Elasticsearch"));
        var elasticsearchSettings = builder.Configuration.GetSection("Elasticsearch").Get<ElasticsearchOptoins>();

        if (elasticsearchSettings != null )
            builder.Services.AddSingleton(provider =>
            {
                var settings = new ElasticsearchClientSettings(new Uri(elasticsearchSettings.Url))
                    .Authentication(new BasicAuthentication(elasticsearchSettings.Username, elasticsearchSettings.Password));

                return new ElasticsearchClient(settings);
            });
        builder.Services.AddScoped<ElasticSearchService>();


        var app = builder.Build();

        var rabbitMQService = app.Services.GetRequiredService<IRabbitMQService>();

        rabbitMQService.ReceiveMessageRpc(RabbitMQService.defaultQueue, RabbitMQService.defaultExchange, ElasticSearchService.TestFunc);

        app.Run();
    }
}