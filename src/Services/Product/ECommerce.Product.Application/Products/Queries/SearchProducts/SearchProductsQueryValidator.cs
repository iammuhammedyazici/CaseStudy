using FluentValidation;

namespace ECommerce.Product.Application.Products.Queries.SearchProducts;

public class SearchProductsQueryValidator : AbstractValidator<SearchProductsQuery>
{
    public SearchProductsQueryValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty().WithMessage("Search query is required")
            .MinimumLength(2).WithMessage("Search query must be at least 2 characters");
    }
}
