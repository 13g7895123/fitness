namespace FitnessTracker.Core.Interfaces;

public interface ILineLoginService
{
    Task<LineUserInfo> ValidateAccessTokenAsync(string accessToken);
    Task<LineTokenResponse> ExchangeCodeForTokenAsync(string code, string redirectUri);
}

public class LineUserInfo
{
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? PictureUrl { get; set; }
}

public class LineTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}
