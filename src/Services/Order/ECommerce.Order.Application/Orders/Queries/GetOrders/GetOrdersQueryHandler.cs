using ECommerce.Contracts.Common;
using ECommerce.Order.Application.Abstractions;
using ECommerce.Order.Application.Abstractions.Persistence;
using ECommerce.Order.Application.Common;
using ECommerce.Order.Application.Orders.Dtos;
using Mapster;
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

        if (string.IsNullOrWhiteSpace(userId))
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

        var orderDtos = orders.Select(o => o.Adapt<GetOrderDto>()).ToList();
        var orderResponses = orderDtos.Select(dto => dto.Adapt<OrderResponse>()).ToList();

        var response = new PagedResponse<OrderResponse>
        {
            Items = orderResponses,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
        };

        return Result<PagedResponse<OrderResponse>>.Success(response);
    }
}
