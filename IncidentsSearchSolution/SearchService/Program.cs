using SearchService.Extensions;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Services.AddCustomHealthChecks();
        builder.Services.AddMessageHandlersService();
        builder.Services.AddRabbitMQServices(builder.Configuration);
        builder.Services.AddElasticsearchServices(builder.Configuration);

        var app = builder.Build();

        app.MapCustomHealthChecks();

        app.InitializeRabbitMQService();

        app.Run();
    }
}