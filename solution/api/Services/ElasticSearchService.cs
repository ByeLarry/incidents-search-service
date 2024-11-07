using Elastic.Clients.Elasticsearch;
using api.Interfaces;
using Elastic.Clients.Elasticsearch.IndexManagement;

namespace api.Services
{
    public sealed class ElasticSearchService(ElasticsearchClient client) : IElasticSearchService
    {
        private readonly ElasticsearchClient _client = client;

        public async Task<IndexResponse> IndexDocumentAsync<T>(T document, string indexName) where T : class
        {
            var response = await _client.IndexAsync(document, i => i.Index(indexName));
            if (!response.IsValidResponse)
            {
                throw new Exception("Document indexing error in Elasticsearch");
            }
            return response;
        }

        public async Task<BulkResponse> IndexManyDocumentsAsync<T>(IEnumerable<T> documents, string indexName) where T : class
        {
            if (!documents.Any())
                await ClearIndexAsync(indexName);

            var response = await _client.IndexManyAsync(documents, indexName);

            if (!response.IsValidResponse)
            {
                throw new Exception("Documents indexing error in Elasticsearch");
            }

            return response;
        }

        public async Task<SearchResponse<T>> SearchAsync<T>(string indexName, string query) where T : class
        {
            var response = await _client.SearchAsync<T>(s => s
                .Index(indexName)
                .Query(q => q.QueryString(qs => qs.Query(query))));

            return response;
        }

        public async Task<DeleteIndexResponse> ClearIndexAsync(string indexName)
        {
            var response = await _client.Indices.DeleteAsync(indexName);
             
            if (!response.IsValidResponse)
            {
                throw new Exception("Failed to clear index in Elasticsearch");
            }

            return response;
        }

        public async Task<DeleteResponse> DeleteDocumentAsync(string documentId, string indexName)
        {
            var response = await _client.DeleteAsync<object>(documentId, d => d.Index(indexName));

            if (!response.IsValidResponse)
            {
                throw new Exception($"Failed to delete document with ID {documentId} in Elasticsearch");
            }

            return response;
        }
    }
}
