using FitnessTracker.Core.Entities;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly ILineLoginService _lineLoginService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly FitnessTrackerDbContext _dbContext;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ILineLoginService lineLoginService,
        IJwtTokenService jwtTokenService,
        FitnessTrackerDbContext dbContext,
        ILogger<AuthController> logger)
    {
        _lineLoginService = lineLoginService;
        _jwtTokenService = jwtTokenService;
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Exchange LINE code for token
            var lineTokenResponse = await _lineLoginService.ExchangeCodeForTokenAsync(
                request.Code,
                request.RedirectUri
            );

            // Get LINE user profile
            var lineUserInfo = await _lineLoginService.ValidateAccessTokenAsync(lineTokenResponse.AccessToken);

            // Check if user exists
            var user = _dbContext.Users.FirstOrDefault(u => u.LineUserId == lineUserInfo.UserId);

            if (user == null)
            {
                // Create new user
                user = new User
                {
                    Id = Guid.NewGuid(),
                    LineUserId = lineUserInfo.UserId,
                    DisplayName = lineUserInfo.DisplayName,
                    PictureUrl = lineUserInfo.PictureUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("新使用者已建立: {UserId}", user.Id);
            }
            else
            {
                // Update user profile
                user.DisplayName = lineUserInfo.DisplayName;
                user.PictureUrl = lineUserInfo.PictureUrl;
                user.UpdatedAt = DateTime.UtcNow;

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("使用者資料已更新: {UserId}", user.Id);
            }

            // Generate JWT token
            var jwtToken = _jwtTokenService.GenerateToken(user.Id, user.LineUserId, user.DisplayName);

            return Ok(new
            {
                accessToken = jwtToken,
                tokenType = "Bearer",
                expiresIn = 86400, // 24 hours in seconds
                user = new
                {
                    id = user.Id,
                    displayName = user.DisplayName,
                    pictureUrl = user.PictureUrl
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登入失敗");
            return BadRequest(new { error = "登入失敗", message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public IActionResult Validate([FromHeader(Name = "Authorization")] string authorization)
    {
        try
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
                return Unauthorized(new { error = "無效的授權標頭" });

            var token = authorization.Substring("Bearer ".Length);
            var (userId, lineUserId, displayName) = _jwtTokenService.ValidateToken(token);

            return Ok(new
            {
                valid = true,
                user = new
                {
                    id = userId,
                    lineUserId,
                    displayName
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "令牌驗證失敗");
            return Unauthorized(new { error = "令牌驗證失敗", message = ex.Message });
        }
    }
}

public class LoginRequest
{
    public string Code { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
}
