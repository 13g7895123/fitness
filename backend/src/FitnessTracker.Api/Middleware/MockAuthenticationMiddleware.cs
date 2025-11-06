using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FitnessTracker.Api.Middleware;

/// <summary>
/// 開發環境用的 Mock 認證中介軟體
/// 允許使用 mock-jwt-token- 開頭的 token 進行測試
/// </summary>
public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public MockAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // 檢查是否有 Authorization header
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        // 檢查是否為 Mock token
        if (!token.StartsWith("mock-jwt-token-"))
        {
            // 不是 Mock token，讓其他認證處理器處理
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        // 創建 Mock 使用者的 Claims
        // 使用固定的 Guid 以便在資料庫中識別
        var mockUserId = "00000000-0000-0000-0000-000000000001";
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, mockUserId),
            new Claim("sub", mockUserId),
            new Claim("lineUserId", "U1234567890abcdef"),
            new Claim("displayName", "測試使用者"),
            new Claim(ClaimTypes.Name, "測試使用者")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        var tokenLog = token.Length > 20 ? token.Substring(0, 20) + "..." : token;
        Logger.LogInformation("Mock authentication successful for token: {Token}", tokenLog);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
