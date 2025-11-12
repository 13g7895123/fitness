using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FitnessTracker.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FitnessTracker.Infrastructure.ExternalServices;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateToken(Guid userId, string lineUserId, string displayName)
    {
        try
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT secret key not configured")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("sub", userId.ToString()),
                new Claim("line_user_id", lineUserId),
                new Claim("display_name", displayName),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate JWT token");
            throw;
        }
    }

    public (Guid UserId, string LineUserId, string DisplayName) ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT secret key not configured")));

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            // 記錄所有的 claims 以便調試
            _logger.LogInformation("Token claims: {Claims}", string.Join(", ", principal.Claims.Select(c => $"{c.Type}={c.Value}")));

            var userIdClaim = principal.FindFirst("sub") ?? principal.FindFirst(ClaimTypes.NameIdentifier);
            var lineUserIdClaim = principal.FindFirst("line_user_id");
            var displayNameClaim = principal.FindFirst("display_name") ?? principal.FindFirst(ClaimTypes.Name);

            if (userIdClaim?.Value == null || lineUserIdClaim?.Value == null || displayNameClaim?.Value == null)
            {
                _logger.LogError("Missing claims - sub: {Sub}, line_user_id: {LineUserId}, display_name: {DisplayName}",
                    userIdClaim?.Value, lineUserIdClaim?.Value, displayNameClaim?.Value);
                throw new SecurityTokenException("Token is missing required claims");
            }

            return (
                Guid.Parse(userIdClaim.Value),
                lineUserIdClaim.Value,
                displayNameClaim.Value
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate JWT token");
            throw;
        }
    }
}
