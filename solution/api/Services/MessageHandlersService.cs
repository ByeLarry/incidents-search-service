using api.Dto;
using api.Interfaces;
using api.Utils;
using System.Text.Json;

namespace api.Services
{
    public sealed class MessageHandlersService(ElasticSearchService es) : IMessageHandlersService
    {
        private readonly ElasticSearchService _es = es;

        public async Task<string> HandleMessage(string message)
        {
            var messageObject = JsonSerializer.Deserialize<MessageDto>(message);

            if (messageObject == null)
                return MessageStatuses.IncorrectMessage;

            var response = await ProtectedSwitchPattern(messageObject);
            Console.WriteLine($"[{messageObject.pattern}] status: {response}");
            return response;
        }

        private async Task<string> ProtectedSwitchPattern(MessageDto messageObject)
        {
            try
            {
                return await SwitchPattern(messageObject);
            }
            catch (Exception ex)  
            { 
                Console.WriteLine(ex.Message);
                return MessageStatuses.IndexingError;
            }
        }

        private async Task<string> SwitchPattern(MessageDto messageDto)
        {
            return messageDto.pattern switch
            {
                SearchMessages.SetMarks => await HandleSetMarks(messageDto.data),
                SearchMessages.SetMark => await HandleSetMark(messageDto.data),
                SearchMessages.GetMarks => await HandleGetMarks(messageDto.data),
                SearchMessages.GetMark => await HandleGetMark(messageDto.data),
                SearchMessages.SetCategories => await HandleSetCategories(messageDto.data),
                SearchMessages.SetCategory => await HandleSetCategory(messageDto.data),
                SearchMessages.GetCategories => await HandleGetCategories(messageDto.data),
                SearchMessages.GetCategory => await HandleGetCategory(messageDto.data),
                SearchMessages.SetUsers => await HandleSetUsers(messageDto.data),
                SearchMessages.SetUser => await HandleSetUser(messageDto.data),
                SearchMessages.GetUsers => await HandleGetUsers(messageDto.data),
                SearchMessages.GetUser =>await HandleGetUser(messageDto.data),
                _ => MessageStatuses.IncorrectPattern
            };
        }

        private async Task<string> HandleSetMarks(JsonElement message)
        {
            if (message.ValueKind != JsonValueKind.Array)
            {
                await _es.ClearIndexAsync(Indeces.Marks);
                return MessageStatuses.IndexCleared;
            }

            var marks = JsonSerializer.Deserialize<MarkDto[]>(message);

            if (marks == null)
                return MessageStatuses.IncorrectData;

            await _es.IndexManyDocumentsAsync(marks, Indeces.Marks);
            return MessageStatuses.Indexed;
        }

        private Task<string> HandleSetMark(JsonElement message)
        {
            return Task.FromResult(MessageStatuses.Indexed);
        }

        private Task<string> HandleGetMarks(JsonElement message)
        {
            return Task.FromResult(MessageStatuses.Indexed);
        }

        private Task<string> HandleGetMark(JsonElement message)
        {
            return Task.FromResult(MessageStatuses.Indexed);
        }

        private async Task<string> HandleSetCategories(JsonElement message)
        {
            var categories = JsonSerializer.Deserialize<CategoryDto[]>(message);

            if (categories == null)
                return MessageStatuses.IncorrectData;

            await _es.IndexManyDocumentsAsync(categories, Indeces.Categories);
            return MessageStatuses.Indexed;
        }

        private async Task<string> HandleSetCategory(JsonElement message)
        {
            var category = JsonSerializer.Deserialize <CategoryDto>(message);

            if (category == null)
                return MessageStatuses.IncorrectData;

            await _es.IndexDocumentAsync(category, Indeces.Categories);
            return MessageStatuses.Indexed;
        }

        private Task<string> HandleGetCategories(JsonElement message)
        {
            return Task.FromResult(MessageStatuses.Indexed);
        }

        private Task<string> HandleGetCategory(JsonElement message)
        {
            return Task.FromResult(MessageStatuses.Indexed);
        }

        private async Task<string> HandleSetUsers(JsonElement message)
        {
            var users = JsonSerializer.Deserialize<UserDto[]>(message);

            if (users == null)
                return MessageStatuses.IncorrectData;

            await _es.IndexManyDocumentsAsync(users, Indeces.Users);
            return MessageStatuses.Indexed;
        }

        private Task<string> HandleSetUser(JsonElement message)
        {
            return Task.FromResult(MessageStatuses.Indexed);
        }

        private Task<string> HandleGetUsers(JsonElement message)
        {
            return Task.FromResult(MessageStatuses.Indexed);
        }

        private Task<string> HandleGetUser(JsonElement message)
        {
            return Task.FromResult(MessageStatuses.Indexed);
        }
    }
}
