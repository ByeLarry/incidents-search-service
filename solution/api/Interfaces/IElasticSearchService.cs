﻿using Elastic.Clients.Elasticsearch;

namespace api.Interfaces
{
    public interface IElasticSearchService
    {
        Task<IndexResponse> IndexDocumentAsync<T>(T document, string indexName) where T : class;
        Task<SearchResponse<T>> SearchAsync<T>(string indexName, string query) where T : class;
    }
}