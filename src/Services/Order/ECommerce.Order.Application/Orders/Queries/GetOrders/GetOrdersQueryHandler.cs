using ECommerce.Contracts.Common;
using ECommerce.Order.Application.Abstractions;
using ECommerce.Order.Application.Abstractions.Persistence;
using ECommerce.Order.Application.Common;
using ECommerce.Order.Application.Orders.Dtos;
using ECommerce.Order.Application.Orders.Queries.GetOrder;
using MediatR;

namespace ECommerce.Order.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, Result<PagedResponse<OrderResponse>>>
{
    private readonly IOrderRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetOrdersQueryHandler(IOrderRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResponse<OrderResponse>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var isAdmin = _currentUserService.IsAdmin;

        var effectiveUserId = isAdmin ? request.UserId : userId;

        if (string.IsNullOrEmpty(userId))
        {
            return Result<PagedResponse<OrderResponse>>.Unauthorized("User is not authenticated");
        }

        var (orders, totalCount) = await _repository.GetPagedOrdersAsync(
            effectiveUserId,
            request.Status,
            request.FromDate,
            request.ToDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var orderDtos = orders.Select(order => new GetOrderDto(
            order.Id,
            order.UserId,
            order.TotalAmount,
            order.Status,
            order.CreatedAt,
            order.Items.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                VariantId = item.VariantId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList(),
            order.Source,
            order.ExternalOrderId,
            order.ExternalSystemCode,
            order.ShippingAddressId,
            order.BillingAddressId,
            order.UpdatedAt,
            order.CancelledAt,
            order.CancellationReason,
            order.CustomerNote
        )).ToList();

        var pagedResult = PagedResult<GetOrderDto>.Create(orderDtos, totalCount, request.PageNumber, request.PageSize);

        var response = new PagedResponse<OrderResponse>
        {
            Items = pagedResult.Items.Select(OrderResponse.FromDto).ToList(),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages,
            HasPreviousPage = pagedResult.HasPreviousPage,
            HasNextPage = pagedResult.HasNextPage
        };

        return Result<PagedResponse<OrderResponse>>.Success(response);
    }
}
