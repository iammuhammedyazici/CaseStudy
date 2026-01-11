using ECommerce.Contracts.Common;
using ECommerce.Stock.Api.Extensions;
using ECommerce.Stock.Application.Stock.Queries.CheckStock;
using ECommerce.Stock.Application.Stock.Queries.GetStock;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Stock.Api.Controllers;

/// <summary>
/// Stock availability and management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StockController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get stock availability for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Stock information</returns>
    /// <response code="200">Stock information found</response>
    /// <response code="404">Product not found</response>
    [HttpGet("{productId:int}")]
    [ProducesResponseType(typeof(GetStockResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStock(int productId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetStockQuery(productId), ct);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Check stock availability for multiple products
    /// </summary>
    /// <param name="query">List of products to check</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Availability for each product</returns>
    /// <response code="200">Stock check completed</response>
    [HttpPost("check")]
    [ProducesResponseType(typeof(List<CheckStockResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckStock([FromBody] CheckStockQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return this.ToActionResult(result);
    }
}
