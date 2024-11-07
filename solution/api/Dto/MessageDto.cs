using Elastic.Clients.Elasticsearch.MachineLearning;
using System.Text.Json;

namespace api.Dto
{
    public class MessageDto
    {
        public required string pattern { get; set; }
        public JsonElement data { get; set; }
        public required string id { get; set; }
    }
}
