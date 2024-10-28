
using api.Interfaces;
using api.Services;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQOptions"));
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();

var app = builder.Build();

var rabbitMQService = app.Services.GetRequiredService<IRabbitMQService>();
rabbitMQService.ReceiveMessage("test", (message) => { Console.WriteLine(message); });

app.Run();

