using System.Text.Json;
using FitnessTracker.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FitnessTracker.Infrastructure.ExternalServices;

public class LineLoginService : ILineLoginService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LineLoginService> _logger;

    public LineLoginService(HttpClient httpClient, IConfiguration configuration, ILogger<LineLoginService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LineUserInfo> ValidateAccessTokenAsync(string accessToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.line.me/v2/profile")
            {
                Headers = { { "Authorization", $"Bearer {accessToken}" } }
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            return new LineUserInfo
            {
                UserId = root.GetProperty("userId").GetString() ?? string.Empty,
                DisplayName = root.GetProperty("displayName").GetString() ?? string.Empty,
                PictureUrl = root.TryGetProperty("pictureUrl", out var pic) ? pic.GetString() : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate LINE access token");
            throw;
        }
    }

    public async Task<LineTokenResponse> ExchangeCodeForTokenAsync(string code, string redirectUri)
    {
        try
        {
            var channelId = _configuration["LineLogin:ChannelId"];
            var channelSecret = _configuration["LineLogin:ChannelSecret"];

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.line.me/oauth2/v2.1/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "redirect_uri", redirectUri },
                    { "client_id", channelId ?? string.Empty },
                    { "client_secret", channelSecret ?? string.Empty }
                })
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            return new LineTokenResponse
            {
                AccessToken = root.GetProperty("access_token").GetString() ?? string.Empty,
                TokenType = root.GetProperty("token_type").GetString() ?? "Bearer",
                ExpiresIn = root.GetProperty("expires_in").GetInt32(),
                RefreshToken = root.TryGetProperty("refresh_token", out var refresh) ? refresh.GetString() ?? string.Empty : string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to exchange LINE authorization code for token");
            throw;
        }
    }
}
