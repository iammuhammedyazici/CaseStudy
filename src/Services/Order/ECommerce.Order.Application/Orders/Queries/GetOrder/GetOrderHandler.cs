using ECommerce.Order.Application.Abstractions.Persistence;
using MediatR;

using ECommerce.Contracts.Common;
using ECommerce.Order.Application.Abstractions;
using Mapster;
using System.Security.Claims;
using ECommerce.Order.Application.Orders.Dtos;

namespace ECommerce.Order.Application.Orders.Queries.GetOrder;

public class GetOrderHandler : IRequestHandler<GetOrderQuery, Result<OrderResponse>>
{
    private readonly IOrderRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetOrderHandler(IOrderRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<OrderResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetOrderAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            return Result<OrderResponse>.NotFound("Order not found");
        }

        var userId = _currentUserService.UserId;
        var isAdmin = _currentUserService.IsAdmin;

        if (order.UserId != userId && !isAdmin)
        {
            return Result<OrderResponse>.Failure("Access denied");
        }

        var dto = order.Adapt<GetOrderDto>();
        var response = dto.Adapt<OrderResponse>();

        return Result<OrderResponse>.Success(response);
    }
}
