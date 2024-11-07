using api.Interfaces;
using api.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void MapCustomHealthChecks(this IEndpointRouteBuilder app)
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecksUI();
        }

        public static void InitializeRabbitMQService(this IApplicationBuilder app)
        {
            var rabbitMQService = app.ApplicationServices.GetRequiredService<IRabbitMQService>();

            var messageHandlersService = app.ApplicationServices.GetRequiredService<MessageHandlersService>();

            rabbitMQService.ReceiveMessageRpc(
                RabbitMQService.defaultQueue,
                RabbitMQService.defaultExchange,
                messageHandlersService.HandleMessage);
        }
    }
}
