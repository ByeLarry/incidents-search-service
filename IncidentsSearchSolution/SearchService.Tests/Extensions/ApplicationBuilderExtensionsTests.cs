using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using SearchService.Extensions;
using SearchService.Interfaces;
using SearchService.Services;
using SearchService.Utils;

namespace SearchService.Tests.Extensions
{
    public class ApplicationBuilderExtensionsTests
    {
        private readonly IApplicationBuilder _appBuilder;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly MessageHandlersService _messageHandlersService;
        private readonly IEndpointRouteBuilder _endpointRouteBuilder;

        public ApplicationBuilderExtensionsTests()
        {
            _rabbitMQService = A.Fake<IRabbitMQService>();
            _messageHandlersService = A.Fake<MessageHandlersService>();

            _serviceProvider = A.Fake<IServiceProvider>();
            A.CallTo(() => _serviceProvider.GetService(typeof(IRabbitMQService))).Returns(_rabbitMQService);
            A.CallTo(() => _serviceProvider.GetService(typeof(MessageHandlersService))).Returns(_messageHandlersService);

            _appBuilder = A.Fake<IApplicationBuilder>();
            A.CallTo(() => _appBuilder.ApplicationServices).Returns(_serviceProvider);

            _endpointRouteBuilder = A.Fake<IEndpointRouteBuilder>();
        }

        [Fact]
        public void InitializeRabbitMQService_ShouldSendAndReceiveMessages()
        {
            _appBuilder.InitializeRabbitMQService();

            A.CallTo(() => _rabbitMQService.SendMessage(A<string>.That.IsNotNull(), Queues.Marks, Queues.Marks, "")).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _rabbitMQService.SendMessage(A<string>.That.IsNotNull(), Queues.Auth, Queues.Auth, "")).MustHaveHappenedOnceExactly();

            A.CallTo(() => _rabbitMQService.ReceiveMessageRpc(
                RabbitMQService.defaultQueue,
                RabbitMQService.defaultExchange,
                _messageHandlersService.HandleMessage)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void InitializeRabbitMQService_ShouldThrowException_WhenRabbitMQServiceIsNotRegistered()
        {
            var appBuilderWithoutRabbitMQ = A.Fake<IApplicationBuilder>();
            var serviceProviderWithoutRabbitMQ = A.Fake<IServiceProvider>();
            A.CallTo(() => serviceProviderWithoutRabbitMQ.GetService(typeof(IRabbitMQService))).Returns(null);
            A.CallTo(() => appBuilderWithoutRabbitMQ.ApplicationServices).Returns(serviceProviderWithoutRabbitMQ);

            var act = () => appBuilderWithoutRabbitMQ.InitializeRabbitMQService();

            act.Should().Throw<InvalidOperationException>("because RabbitMQService is not registered in the service container");
        }


        [Fact]
        public void InitializeRabbitMQService_ShouldThrowException_WhenMessageHandlersServiceIsNotRegistered()
        {
            var appBuilderMock = A.Fake<IApplicationBuilder>();
            var serviceProviderMock = A.Fake<IServiceProvider>();
            var rabbitMQServiceMock = A.Fake<IRabbitMQService>();
            A.CallTo(() => serviceProviderMock.GetService(typeof(IRabbitMQService)))
                .Returns(rabbitMQServiceMock);
            A.CallTo(() => serviceProviderMock.GetService(typeof(MessageHandlersService)))
                .Returns(null);
            A.CallTo(() => appBuilderMock.ApplicationServices)
                .Returns(serviceProviderMock);

            var act = () => appBuilderMock.InitializeRabbitMQService();

            act.Should()
               .Throw<InvalidOperationException>("because MessageHandlersService is not registered in the service container")
               .WithMessage("*MessageHandlersService*");
        }
    }
}
