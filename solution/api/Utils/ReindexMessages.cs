using api.Dto;
using System.Text.Json;

namespace api.Utils
{
    public static class ReindexMessages
    {
        public static string GetSerializedTemplate(string _pattern)
        {
            return JsonSerializer.Serialize(GetTemplate(_pattern));
        }

        public static MessageDto GetTemplate(string _pattern) 
        {
            return new MessageDto()
            {
                pattern = _pattern,
                id = Guid.NewGuid().ToString(),
                data = CreateEmptyJsonElement()
            };
        }

        private static JsonElement CreateEmptyJsonElement()
        {
            using var doc = JsonDocument.Parse("{}"); 
            return doc.RootElement.Clone(); 
        }
    }
}