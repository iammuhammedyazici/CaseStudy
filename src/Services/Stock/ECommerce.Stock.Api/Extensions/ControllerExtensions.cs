using ECommerce.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Stock.Api.Extensions;

public static class ControllerExtensions
{
    public static IActionResult ToActionResult(this ControllerBase controller, Result result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(new { message = "Operation completed successfully" });
        }

        return result.Type switch
        {
            ResultType.NotFound => controller.NotFound(new { error = result.Error }),
            ResultType.BadRequest => controller.BadRequest(new { error = result.Error }),
            ResultType.Conflict => controller.Conflict(new { error = result.Error }),
            ResultType.Invalid => controller.BadRequest(new { error = result.Error }),
            ResultType.Unauthorized => controller.Unauthorized(new { error = result.Error }),
            ResultType.Forbidden => controller.Forbid(),
            ResultType.UnprocessableEntity => controller.UnprocessableEntity(new { error = result.Error }),
            _ => controller.StatusCode(500, new { error = "An unexpected error occurred." })
        };
    }

    public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        return result.Type switch
        {
            ResultType.NotFound => controller.NotFound(new { error = result.Error }),
            ResultType.BadRequest => controller.BadRequest(new { error = result.Error }),
            ResultType.Conflict => controller.Conflict(new { error = result.Error }),
            ResultType.Invalid => controller.BadRequest(new { error = result.Error }),
            ResultType.Unauthorized => controller.Unauthorized(new { error = result.Error }),
            ResultType.Forbidden => controller.Forbid(),
            ResultType.UnprocessableEntity => controller.UnprocessableEntity(new { error = result.Error }),
            _ => controller.StatusCode(500, new { error = "An unexpected error occurred." })
        };
    }
}
