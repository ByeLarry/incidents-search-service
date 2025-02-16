using SearchService.Extensions;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Services.AddCustomHealthChecks(builder.Configuration);
        builder.Services.AddMessageHandlersService();
        builder.Services.AddRabbitMQServices(builder.Configuration);
        builder.Services.AddElasticsearchServices(builder.Configuration);

        var app = builder.Build();

		app.MapGet("/", () => Results.Ok("Service is up and running"));

		app.MapCustomHealthChecks();

        app.InitializeRabbitMQService();

        app.Run();
    }
}