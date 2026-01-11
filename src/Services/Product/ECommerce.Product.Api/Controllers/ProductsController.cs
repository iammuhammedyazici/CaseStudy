using ECommerce.Product.Application.Products.Queries.GetProductById;
using ECommerce.Product.Application.Products.Queries.GetProducts;
using ECommerce.Product.Application.Products.Queries.SearchProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Product.Api.Controllers;

/// <summary>
/// Product catalog and search operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// List all products
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of products</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var products = await _mediator.Send(new GetProductsQuery(), ct);
        return Ok(products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Product details</returns>
    /// <response code="200">Product found</response>
    /// <response code="404">Product not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id), ct);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    /// <summary>
    /// Search products by name or description
    /// </summary>
    /// <param name="q">Search query</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Matching products</returns>
    /// <response code="200">Search results</response>
    /// <response code="400">Query is empty</response>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct)
    {
        var results = await _mediator.Send(new SearchProductsQuery(q), ct);
        return Ok(results);
    }
}
