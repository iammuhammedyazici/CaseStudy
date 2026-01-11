using ECommerce.Order.Api.Extensions;
using ECommerce.Order.Application.Orders.Commands.CancelOrder;
using ECommerce.Order.Application.Orders.Commands.CreateOrder;
using ECommerce.Order.Application.Orders.Dtos;
using ECommerce.Order.Application.Orders.Queries.GetOrder;
using ECommerce.Order.Application.Orders.Queries.GetOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Order.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new order for the authenticated customer.
    /// </summary>
    /// <remarks>
    /// This endpoint creates a new order based on the provided request data and the authenticated user context.
    /// <b>Authentication:</b> Required (JWT Bearer Token)  
    /// <b>Authorization:</b> Customer role
    /// <b>Behavior:</b>
    /// - UserId is automatically resolved from the JWT token
    /// - Order is created with initial status <i>PendingStock</i>
    /// - An <i>OrderCreatedEvent</i> is published to RabbitMQ for downstream processes
    /// 
    /// <b>Validation Rules:</b>
    /// - Shipping address must be provided
    /// - Marketplace orders require external order information
    /// </remarks>
    /// <param name="command">Order creation request payload</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The created order result</returns>

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Retrieves a single order by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Returns detailed information about the specified order.
    /// 
    /// Access is restricted to:
    /// - The owner of the order
    /// - Authorized system roles (e.g. Admin)
    /// </remarks>
    /// <param name="id">Unique order identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Order details if found</returns>


    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrderQuery(id), ct);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Retrieves a paginated list of orders for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Supports pagination, filtering and sorting through query parameters.
    /// 
    /// Typical use cases:
    /// - Order history listing
    /// - Admin or customer dashboards
    /// </remarks>
    /// <param name="query">Filtering and pagination parameters</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A paginated list of orders</returns>

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(PagedResponse<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders([FromQuery] GetOrdersQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Cancels an existing order.
    /// </summary>
    /// <remarks>
    /// Attempts to cancel the specified order based on business rules.
    /// 
    /// <b>Rules:</b>
    /// - Only cancellable order statuses are allowed
    /// - User must be the owner of the order or have sufficient permissions
    /// 
    /// If cancellation is successful, related domain events may be published.
    /// </remarks>
    /// <param name="id">Unique order identifier</param>
    /// <param name="command">Cancellation reason and metadata</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Cancellation operation result</returns>

    [HttpPatch("{id:guid}/cancel")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderCommand command, CancellationToken ct)
    {
        var finalCommand = command with { OrderId = id };
        var result = await _mediator.Send(finalCommand, ct);
        return this.ToActionResult(result);
    }
}
