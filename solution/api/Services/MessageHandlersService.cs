using api.Dto;
using api.Interfaces;
using api.Utils;
using System.Text.Json;

namespace api.Services
{
    public sealed class MessageHandlersService(ElasticSearchService es, ILogger<MessageHandlersService> logger) : IMessageHandlersService
    {
        private readonly ElasticSearchService _es = es;
        private readonly ILogger<MessageHandlersService> _logger = logger;

        public async Task<string> HandleMessage(string message)
        {
            var messageObject = JsonSerializer.Deserialize<MessageDto>(message);
            return messageObject == null ? MessageStatuses.IncorrectMessage : await ProtectedSwitchPattern(messageObject);
        }

        private async Task<string> ProtectedSwitchPattern(MessageDto messageObject)
        {
            try
            {
                return await SwitchPattern(messageObject);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Argument null exception while processing message with pattern: {Pattern}", messageObject.pattern);
                return MessageStatuses.SearchingError;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception while processing message with pattern: {Pattern}", messageObject.pattern);
                return MessageStatuses.IncorrectData;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation exception while processing message with pattern: {Pattern}", messageObject.pattern);
                return MessageStatuses.IndexingError;
            }
        }

        private async Task<string> SwitchPattern(MessageDto messageDto)
        {
            return messageDto.pattern switch
            {
                SearchMessages.SetMarks => await HandleSetAsync<MarkDto>(messageDto.data, Indeces.Marks),
                SearchMessages.SetMark => await HandleSingleIndexAsync<MarkDto>(messageDto.data, Indeces.Marks),
                SearchMessages.DeleteMark => await HandleDeleteAsync<MarkDto>(messageDto.data, Indeces.Marks),
                SearchMessages.SearchMarks => await HandleSearchAsync<MarkDto>(messageDto.data, Indeces.Marks, new[] { "title", "description", "addressName", "addressDescription" }),

                SearchMessages.SetCategories => await HandleSetAsync<CategoryDto>(messageDto.data, Indeces.Categories),
                SearchMessages.SetCategory => await HandleSingleIndexAsync<CategoryDto>(messageDto.data, Indeces.Categories),
                SearchMessages.DeleteCategory => await HandleDeleteAsync<CategoryDto>(messageDto.data, Indeces.Categories),
                SearchMessages.SearchCategories => await HandleSearchAsync<CategoryDto>(messageDto.data, Indeces.Categories, new[] { "name" }),

                SearchMessages.SetUsers => await HandleSetAsync<UserDto>(messageDto.data, Indeces.Users),
                SearchMessages.SetUser => await HandleSingleIndexAsync<UserDto>(messageDto.data, Indeces.Users),
                SearchMessages.DeleteUser => await HandleDeleteAsync<UserDto>(messageDto.data, Indeces.Users),
                SearchMessages.SearchUsers => await HandleSearchAsync<UserDto>(messageDto.data, Indeces.Users, new[] { "id.keyword", "email.keyword", "name", "surname" }),

                _ => MessageStatuses.IncorrectPattern
            };
        }

        private async Task<string> HandleSetAsync<T>(JsonElement message, string indexName) where T : class
        {
            await _es.ClearIndexAsync(indexName);

            var items = DeserializeArray<T>(message);
            if (items == null)
            {
                _logger.LogWarning("Deserialization returned null while processing index: {IndexName}", indexName);
                return MessageStatuses.IncorrectData;
            }

            await _es.IndexManyDocumentsAsync(items, indexName);

            _logger.LogInformation(
                "Successfully indexed {Count} items to index: {IndexName}",
                items.Length,
                indexName
            );

            return MessageStatuses.Indexed;
        }

        private async Task<string> HandleSingleIndexAsync<T>(JsonElement message, string indexName) where T : class
        {
            var item = JsonSerializer.Deserialize<T>(message);
            if (item == null)
                return MessageStatuses.IncorrectData;

            await _es.IndexDocumentAsync(item, indexName);
            return MessageStatuses.Indexed;
        }

        private async Task<string> HandleDeleteAsync<T>(JsonElement message, string indexName) where T : class
        {
            var item = JsonSerializer.Deserialize<T>(message);
            if (item == null)
                return MessageStatuses.IncorrectData;

            var id = typeof(T).GetProperty("id")?.GetValue(item)?.ToString();
            if (string.IsNullOrEmpty(id))
                return MessageStatuses.IncorrectData;

            await _es.DeleteDocumentAsync(id, indexName);
            return MessageStatuses.Deleted;
        }

        private async Task<string> HandleSearchAsync<T>(JsonElement message, string indexName, string[] searchFields) where T : class
        {
            var search = JsonSerializer.Deserialize<SearchDto>(message);
            if (search == null)
                throw new ArgumentNullException(nameof(search));

            var response = await _es.SearchAsync<T>(search.index, search.query, searchFields);
            return JsonSerializer.Serialize(response.Hits.Select(hit => hit.Source).ToList());
        }

        private T[]? DeserializeArray<T>(JsonElement element) => JsonSerializer.Deserialize<T[]>(element);
    }
}
