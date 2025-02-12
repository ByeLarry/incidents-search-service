using SearchService.Interfaces;
using SearchService.Services;
using SearchService.Utils;

namespace SearchService.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void MapCustomHealthChecks(this IEndpointRouteBuilder app)
        {
            app.MapHealthChecks("/health");
        }

        public static void InitializeRabbitMQService(this IApplicationBuilder app)
        {
            var rabbitMQService = app.ApplicationServices.GetRequiredService<IRabbitMQService>();

            var messageHandlersService = app.ApplicationServices.GetRequiredService<MessageHandlersService>();

            rabbitMQService.SendMessage(ReindexMessages.GetSerializedTemplate(RMQPatterns.Marks), Queues.Marks, Queues.Marks);
            rabbitMQService.SendMessage(ReindexMessages.GetSerializedTemplate(RMQPatterns.Users), Queues.Auth, Queues.Auth);
            rabbitMQService.SendMessage(ReindexMessages.GetSerializedTemplate(RMQPatterns.Categories), Queues.Marks, Queues.Marks);

            rabbitMQService.ReceiveMessageRpc(
                RabbitMQService.defaultQueue,
                RabbitMQService.defaultExchange,
                messageHandlersService.HandleMessage);
        }
    }
}
