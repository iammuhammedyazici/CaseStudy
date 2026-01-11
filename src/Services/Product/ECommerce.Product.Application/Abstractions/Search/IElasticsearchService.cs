namespace ECommerce.Product.Application.Abstractions.Search;

public interface IElasticsearchService
{
    Task IndexProductAsync(Domain.Entities.Product product, CancellationToken cancellationToken);
    Task IndexProductsAsync(IEnumerable<Domain.Entities.Product> products, CancellationToken cancellationToken);
    Task<List<Domain.Entities.Product>> SearchProductsAsync(string query, CancellationToken cancellationToken);
}
