using Elastic.Clients.Elasticsearch;
using SearchService.Interfaces;
using Elastic.Clients.Elasticsearch.IndexManagement;
using SearchService.Dto;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport.Products.Elasticsearch;
using System.Text.RegularExpressions;

namespace SearchService.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly ElasticsearchClient _client;

        public ElasticSearchService(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<IndexResponse> IndexDocumentAsync<T>(T document, string indexName) where T : class
        {
            var response = await _client.IndexAsync(document, i => i.Index(indexName));
            EnsureValidResponse(response, "Document indexing error in Elasticsearch");
            return response;
        }

        public async Task<BulkResponse> IndexManyDocumentsAsync<T>(IEnumerable<T> documents, string indexName) where T : class
        {
            if (!documents.Any())
                await ClearIndexAsync(indexName);

            var response = await _client.IndexManyAsync(documents, indexName);
            EnsureValidResponse(response, "Documents indexing error in Elasticsearch");

            return response;
        }

        public async Task<SearchResponse<T>> SearchAsync<T>(string indexName, string query, string[] fields) where T : class
        {
            var response = await _client.SearchAsync<T>(s => s
                .Index(indexName)
                .SourceIncludes(new[] { "id" })           
                .Query(q => q.Bool(b => b
                    .Should(
                        sh => sh.QueryString(qs => qs
                            .Fields(fields)
                            .Query($"*{EscapeSpecialCharacters(query)}*")
                            .AnalyzeWildcard(true)
                            .DefaultOperator(Operator.And)
                        )
                    )
                )));
            return response;
        }

        public async Task<DeleteIndexResponse> ClearIndexAsync(string indexName)
        {
            return await _client.Indices.DeleteAsync(indexName);
        }

        public async Task<DeleteResponse> DeleteDocumentAsync(string documentId, string indexName)
        {
            var response = await _client.DeleteAsync<object>(documentId, d => d.Index(indexName));
            EnsureValidResponse(response, $"Failed to delete document with ID {documentId} in Elasticsearch");

            return response;
        }

        private static void EnsureValidResponse(ElasticsearchResponse response, string errorMessage)
        {
            if (!response.IsValidResponse)
            {
                throw new InvalidOperationException(errorMessage);
            }
        }

        private string EscapeSpecialCharacters(string query)
        {
            return Regex.Replace(query, @"([+\-!(){}\[\]^""~*?:\\/])", @"\$1");
        }
    }
}
