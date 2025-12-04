using FitnessTracker.Api.Controllers;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.Common;
using FitnessTracker.Shared.Dtos.Goals;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FitnessTracker.UnitTests.Controllers;

public class GoalsControllerTests
{
    private readonly Mock<IGoalService> _mockService;
    private readonly GoalsController _controller;
    private readonly Guid _testUserId = Guid.NewGuid();

    public GoalsControllerTests()
    {
        _mockService = new Mock<IGoalService>();
        _controller = new GoalsController(_mockService.Object);
        SetupUserContext();
    }

    private void SetupUserContext()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task Create_WithValidDto_ReturnsOk()
    {
        // Arrange
        var createDto = new CreateGoalDto { Name = "減重目標", TargetValue = 5, Unit = "kg" };
        var createdGoal = new GoalDto { Id = 1, Name = "減重目標", TargetValue = 5, Unit = "kg" };
        _mockService.Setup(s => s.CreateAsync(createDto, _testUserId)).ReturnsAsync(createdGoal);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<GoalDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Name.Should().Be("減重目標");
    }

    [Fact]
    public async Task Update_WithValidId_ReturnsOk()
    {
        // Arrange
        var updateDto = new UpdateGoalDto { Name = "更新目標", TargetValue = 10 };
        var updatedGoal = new GoalDto { Id = 1, Name = "更新目標", TargetValue = 10, Unit = "kg" };
        _mockService.Setup(s => s.UpdateAsync(1, updateDto, _testUserId)).ReturnsAsync(updatedGoal);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<GoalDto>>().Subject;
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Update_WithUnauthorizedAccess_ReturnsUnauthorized()
    {
        // Arrange
        var updateDto = new UpdateGoalDto { Name = "更新目標", TargetValue = 10 };
        _mockService.Setup(s => s.UpdateAsync(1, updateDto, _testUserId))
            .ThrowsAsync(new UnauthorizedAccessException("無權限"));

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Delete_WithValidId_ReturnsOk()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(1, _testUserId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOk()
    {
        // Arrange
        var goal = new GoalDto { Id = 1, Name = "目標", TargetValue = 5, Unit = "kg" };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(goal);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<GoalDto>>().Subject;
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((GoalDto?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithGoals()
    {
        // Arrange
        var goals = new List<GoalDto>
        {
            new() { Id = 1, Name = "目標1", TargetValue = 5, Unit = "kg" },
            new() { Id = 2, Name = "目標2", TargetValue = 10, Unit = "kg" }
        };
        _mockService.Setup(s => s.GetAllByUserAsync(_testUserId)).ReturnsAsync(goals);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<GoalDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetActive_ReturnsOkWithActiveGoals()
    {
        // Arrange
        var goals = new List<GoalDto>
        {
            new() { Id = 1, Name = "活躍目標", TargetValue = 5, Unit = "kg", IsCompleted = false }
        };
        _mockService.Setup(s => s.GetActiveByUserAsync(_testUserId)).ReturnsAsync(goals);

        // Act
        var result = await _controller.GetActive();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<GoalDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateProgress_WithValidId_ReturnsOk()
    {
        // Arrange
        _mockService.Setup(s => s.UpdateGoalProgressAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateProgress(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        response.Success.Should().BeTrue();
    }
}
