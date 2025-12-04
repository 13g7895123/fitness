using System.Security.Claims;
using FitnessTracker.Shared.Dtos.Common;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.Api.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("無法獲取使用者資訊");
            }

            return userId;
        }

        protected ActionResult<ApiResponse<T>> Success<T>(T data, string message = null)
        {
            return Ok(ApiResponse<T>.SuccessResponse(data, message));
        }

        protected ActionResult<ApiResponse<T>> Created<T>(string actionName, object routeValues, T data, string message = null)
        {
            return CreatedAtAction(actionName, routeValues, ApiResponse<T>.SuccessResponse(data, message));
        }
    }
}
