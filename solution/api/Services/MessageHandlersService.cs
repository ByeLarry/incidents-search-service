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

            return await ProtectedSwitchPattern(messageObject);
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
                SearchMessages.DeleteMark => await HandleDeleteMark(messageDto.data),
                SearchMessages.SetCategories => await HandleSetCategories(messageDto.data),
                SearchMessages.SetCategory => await HandleSetCategory(messageDto.data),
                SearchMessages.DeleteCategory => await HandleDeleteCategory(messageDto.data),
                SearchMessages.SetUsers => await HandleSetUsers(messageDto.data),
                SearchMessages.SetUser => await HandleSetUser(messageDto.data),
                SearchMessages.DeleteUser => await HandleDeleteUser(messageDto.data),
                _ => MessageStatuses.IncorrectPattern
            };
        }

        private async Task<string> HandleSetMarks(JsonElement message)
        {
            
            await _es.ClearIndexAsync(Indeces.Marks);
         
            var marks = JsonSerializer.Deserialize<MarkDto[]>(message);

            if (marks == null)
                return MessageStatuses.IncorrectData;

            await _es.IndexManyDocumentsAsync(marks, Indeces.Marks);
            return MessageStatuses.Indexed;
        }

        private async Task<string> HandleSetMark(JsonElement message)
        {
            var mark = JsonSerializer.Deserialize<MarkDto>(message);

            if (mark == null)
                return MessageStatuses.IncorrectData;

            await _es.IndexDocumentAsync(mark, Indeces.Marks);
            return MessageStatuses.Indexed;
        }

        private async Task<string> HandleDeleteMark(JsonElement message)
        {
            var mark = JsonSerializer.Deserialize<MarkDto>(message);

            if (mark == null)
                return MessageStatuses.IncorrectData;

            await _es.DeleteDocumentAsync(mark.id.ToString(), Indeces.Marks);
            return MessageStatuses.Deleted;
        }

        private async Task<string> HandleSetCategories(JsonElement message)
        {
            await _es.ClearIndexAsync(Indeces.Categories);
             
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

        private async Task<string> HandleDeleteCategory(JsonElement message)
        {
            var category = JsonSerializer.Deserialize<CategoryDto>(message);

            if (category == null)
                return MessageStatuses.IncorrectData;

            await _es.DeleteDocumentAsync(category.id.ToString(), Indeces.Categories);
            return MessageStatuses.Deleted;
        }

        private async Task<string> HandleSetUsers(JsonElement message)
        {
            await _es.ClearIndexAsync(Indeces.Users);

            var users = JsonSerializer.Deserialize<UserDto[]>(message);

            if (users == null)
                return MessageStatuses.IncorrectData;

            await _es.IndexManyDocumentsAsync(users, Indeces.Users);
            return MessageStatuses.Indexed;
        }

        private async Task<string> HandleSetUser(JsonElement message)
        {
            var user = JsonSerializer.Deserialize<UserDto>(message);

            if (user == null)
                return MessageStatuses.IncorrectData;

            await _es.IndexDocumentAsync(user, Indeces.Users);
            return MessageStatuses.Indexed;
        }

        private async Task<string> HandleDeleteUser(JsonElement message)
        {
            var user = JsonSerializer.Deserialize<UserDto>(message);

            if (user == null)
                return MessageStatuses.IncorrectData;

            await _es.DeleteDocumentAsync(user.id.ToString(), Indeces.Users);
            return MessageStatuses.Deleted;
        }
    }
}
