using Elastic.Clients.Elasticsearch;

namespace api.Services
{
    public class ElasticSearchService
    {
        private readonly ElasticsearchClient _client;

        public ElasticSearchService(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task IndexDocumentAsync<T>(T document, string indexName) where T : class
        {
            var response = await _client.IndexAsync(document, i => i.Index(indexName));
            if (!response.IsValidResponse)
            {
                throw new Exception("Ошибка индексации документа в Elasticsearch");
            }
        }

        public async Task<SearchResponse<T>> SearchAsync<T>(string indexName, string query) where T : class
        {
            var response = await _client.SearchAsync<T>(s => s
                .Index(indexName)
                .Query(q => q.QueryString(qs => qs.Query(query))));

            return response;
        }
        public static string TestFunc(string val)
        {
            return "hello from c#";
        }
    }
}
