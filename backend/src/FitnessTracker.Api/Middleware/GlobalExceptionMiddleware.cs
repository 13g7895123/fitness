using System.Net;
using FitnessTracker.Core.Exceptions;
using FitnessTracker.Shared.Dtos.Common;
using Microsoft.IdentityModel.Tokens;

namespace FitnessTracker.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var apiResponse = new ApiResponse<object>();

        switch (exception)
        {
            case NotFoundException nfe:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                apiResponse = ApiResponse<object>.FailureResponse(nfe.Message);
                break;

            case ValidationException ve:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errors = ve.Errors.Select(e => new ApiError
                {
                    Code = "VALIDATION_ERROR",
                    Field = e.Key,
                    Message = string.Join(", ", e.Value)
                }).ToList();
                apiResponse = ApiResponse<object>.ErrorResponse(ve.Message, errors);
                break;

            case BusinessException be:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                apiResponse = ApiResponse<object>.ErrorResponse(be.Message, new List<ApiError>
                {
                    new ApiError { Code = be.ErrorCode, Message = be.Message }
                });
                break;

            case SecurityTokenException ste:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                apiResponse = ApiResponse<object>.FailureResponse("令牌無效或已過期");
                break;

            case ArgumentException ae:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                apiResponse = ApiResponse<object>.FailureResponse(ae.Message);
                break;

            case UnauthorizedAccessException uae:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                apiResponse = ApiResponse<object>.FailureResponse("您沒有權限執行此操作");
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse = ApiResponse<object>.FailureResponse("發生內部伺服器錯誤，請稍後再試");
                break;
        }

        return response.WriteAsJsonAsync(apiResponse);
    }
}
