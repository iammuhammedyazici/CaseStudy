using ECommerce.Product.Application.Abstractions.Search;
using Nest;

namespace ECommerce.Product.Infrastructure.Search;

public class ElasticsearchService : IElasticsearchService
{
    private readonly IElasticClient _client;
    private const string IndexName = "products";

    public ElasticsearchService(IElasticClient client)
    {
        _client = client;
        InitializeIndexAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeIndexAsync()
    {
        var existsResponse = await _client.Indices.ExistsAsync(IndexName);
        if (!existsResponse.Exists)
        {
            var createResponse = await _client.Indices.CreateAsync(IndexName, c => c
                .Map<Domain.Entities.Product>(m => m
                    .AutoMap()
                    .Properties(p => p
                        .Text(t => t.Name(n => n.Name).Analyzer("standard"))
                        .Text(t => t.Name(n => n.Description).Analyzer("standard"))
                        .Keyword(k => k.Name(n => n.Category))
                        .Keyword(k => k.Name(n => n.Brand))
                        .Nested<Domain.Entities.ProductVariant>(n => n
                            .Name(nn => nn.Variants)
                            .AutoMap()
                        )
                    )
                )
            );
        }
    }

    public async Task IndexProductAsync(Domain.Entities.Product product, CancellationToken cancellationToken)
    {
        var response = await _client.IndexDocumentAsync(product);
        if (!response.IsValid)
        {
            throw new Exception($"Failed to index product: {response.OriginalException?.Message}");
        }
    }

    public async Task IndexProductsAsync(IEnumerable<Domain.Entities.Product> products, CancellationToken cancellationToken)
    {
        var bulkResponse = await _client.BulkAsync(b => b
            .Index(IndexName)
            .IndexMany(products)
        );

        if (!bulkResponse.IsValid)
        {
            throw new Exception($"Failed to bulk index products: {bulkResponse.OriginalException?.Message}");
        }
    }

    public async Task<List<Domain.Entities.Product>> SearchProductsAsync(string query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<Domain.Entities.Product>();
        }

        query = SanitizeSearchQuery(query);

        if (query.Length > 100)
        {
            query = query.Substring(0, 100);
        }

        var searchResponse = await _client.SearchAsync<Domain.Entities.Product>(s => s
            .Index(IndexName)
            .Query(q => q
                .MultiMatch(m => m
                    .Query(query)
                    .Fields(f => f
                        .Field(ff => ff.Name, boost: 2.0)
                        .Field(ff => ff.Description)
                        .Field(ff => ff.Category)
                        .Field(ff => ff.Brand)
                    )
                    .Fuzziness(Fuzziness.Auto)
                )
            )
            .Size(100)
        );

        return searchResponse.Documents.ToList();
    }

    private static string SanitizeSearchQuery(string query)
    {
        var specialChars = new[] { "\\", "+", "-", "=", "&&", "||", ">", "<", "!", "(", ")", "{", "}", "[", "]", "^", "\"", "~", "*", "?", ":", "/" };

        foreach (var ch in specialChars)
        {
            query = query.Replace(ch, " ");
        }

        while (query.Contains("  "))
        {
            query = query.Replace("  ", " ");
        }

        return query.Trim();
    }
}
