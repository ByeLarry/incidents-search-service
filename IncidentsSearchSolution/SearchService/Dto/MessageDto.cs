using System.Text.Json;

namespace SearchService.Dto
{
    public class MessageDto
    {
        public required string pattern { get; set; }
        public JsonElement data { get; set; }
        public required string id { get; set; }
    }
}
