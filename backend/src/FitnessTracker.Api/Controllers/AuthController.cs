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
    private readonly IConfiguration _configuration;

    public AuthController(
        ILineLoginService lineLoginService,
        IJwtTokenService jwtTokenService,
        FitnessTrackerDbContext dbContext,
        ILogger<AuthController> logger,
        IConfiguration configuration)
    {
        _lineLoginService = lineLoginService;
        _jwtTokenService = jwtTokenService;
        _dbContext = dbContext;
        _logger = logger;
        _configuration = configuration;
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

    [HttpPost("test-login")]
    public async Task<IActionResult> TestLogin()
    {
        try
        {
            // 檢查是否啟用測試登入功能
            var enableTestLogin = _configuration.GetValue<bool>("EnableTestLogin", true);
            if (!enableTestLogin)
            {
                _logger.LogWarning("測試登入功能已停用");
                return BadRequest(new { error = "測試登入功能未啟用" });
            }

            // 測試使用者的固定 ID
            var testUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            // 從資料庫查找測試使用者
            var testUser = await _dbContext.Users.FindAsync(testUserId);

            if (testUser == null)
            {
                _logger.LogError("測試使用者不存在於資料庫");
                return NotFound(new { error = "測試使用者不存在，請確保資料庫已正確初始化" });
            }

            // 生成真實的 JWT token
            var jwtToken = _jwtTokenService.GenerateToken(
                testUser.Id,
                testUser.LineUserId,
                testUser.DisplayName
            );

            _logger.LogWarning("測試帳號登入: {DisplayName} (ID: {UserId})", testUser.DisplayName, testUser.Id);

            return Ok(new
            {
                accessToken = jwtToken,
                tokenType = "Bearer",
                expiresIn = 86400, // 24 hours in seconds
                user = new
                {
                    id = testUser.Id,
                    lineUserId = testUser.LineUserId,
                    displayName = testUser.DisplayName,
                    pictureUrl = testUser.PictureUrl
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "測試登入失敗");
            return StatusCode(500, new { error = "測試登入失敗", message = ex.Message });
        }
    }
}

public class LoginRequest
{
    public string Code { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
}
