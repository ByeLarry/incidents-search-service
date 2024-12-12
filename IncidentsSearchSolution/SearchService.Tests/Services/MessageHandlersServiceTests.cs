using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using SearchService.Services;
using SearchService.Utils;

namespace SearchService.Tests.Services
{

    public class MessageHandlersServiceTests
    {
        private readonly ElasticSearchService _fakeElasticSearchService;
        private readonly ILogger<MessageHandlersService> _fakeLogger;
        private readonly MessageHandlersService _service;

        public MessageHandlersServiceTests()
        {
            _fakeElasticSearchService = A.Fake<ElasticSearchService>();
            _fakeLogger = A.Fake<ILogger<MessageHandlersService>>();
            _service = new MessageHandlersService(_fakeElasticSearchService, _fakeLogger);
        }

        [Fact]
        public async Task HandleMessage_ShouldReturnIncorrectMessage_WhenMessageIsNull()
        {
            var result = await _service.HandleMessage(null);

            result.Should().Be(MessageStatuses.IncorrectMessage);
        }

        [Fact]
        public async Task HandleMessage_ShouldReturnIncorrectMessage_WhenDeserializationFails()
        {
            var invalidMessage = "Invalid Message";

            var result = await _service.HandleMessage(invalidMessage);

            result.Should().Be(MessageStatuses.IncorrectMessage);
        }
    }

}
