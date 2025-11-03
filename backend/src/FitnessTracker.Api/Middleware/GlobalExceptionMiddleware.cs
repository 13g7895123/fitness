using System.Net;
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
