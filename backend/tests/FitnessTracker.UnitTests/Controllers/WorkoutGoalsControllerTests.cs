using FitnessTracker.Api.Controllers;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.Common;
using FitnessTracker.Shared.Dtos.WorkoutRecords;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FitnessTracker.UnitTests.Controllers;

public class WorkoutGoalsControllerTests
{
    private readonly Mock<IWorkoutGoalService> _mockService;
    private readonly WorkoutGoalsController _controller;
    private readonly Guid _testUserId = Guid.NewGuid();

    public WorkoutGoalsControllerTests()
    {
        _mockService = new Mock<IWorkoutGoalService>();
        _controller = new WorkoutGoalsController(_mockService.Object);
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
        var createDto = new CreateWorkoutGoalDto { WeeklyMinutes = 150, WeeklyCalories = 1000 };
        var createdGoal = new WorkoutGoalDto 
        { 
            Id = Guid.NewGuid(), 
            UserId = _testUserId,
            WeeklyMinutes = 150, 
            WeeklyCalories = 1000,
            IsActive = true
        };
        _mockService.Setup(s => s.CreateAsync(createDto, _testUserId)).ReturnsAsync(createdGoal);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<WorkoutGoalDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.WeeklyMinutes.Should().Be(150);
    }

    [Fact]
    public async Task Update_WithValidId_ReturnsOk()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var updateDto = new UpdateWorkoutGoalDto { WeeklyMinutes = 200, WeeklyCalories = 1500, IsActive = true };
        var updatedGoal = new WorkoutGoalDto 
        { 
            Id = goalId, 
            UserId = _testUserId,
            WeeklyMinutes = 200, 
            WeeklyCalories = 1500,
            IsActive = true
        };
        _mockService.Setup(s => s.UpdateAsync(goalId, updateDto, _testUserId)).ReturnsAsync(updatedGoal);

        // Act
        var result = await _controller.Update(goalId, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<WorkoutGoalDto>>().Subject;
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var updateDto = new UpdateWorkoutGoalDto { WeeklyMinutes = 200 };
        _mockService.Setup(s => s.UpdateAsync(goalId, updateDto, _testUserId))
            .ThrowsAsync(new KeyNotFoundException("目標不存在"));

        // Act
        var result = await _controller.Update(goalId, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_WithValidId_ReturnsOk()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(goalId, _testUserId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(goalId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(goalId, _testUserId))
            .ThrowsAsync(new KeyNotFoundException("目標不存在"));

        // Act
        var result = await _controller.Delete(goalId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOk()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var goal = new WorkoutGoalDto { Id = goalId, UserId = _testUserId, WeeklyMinutes = 150 };
        _mockService.Setup(s => s.GetByIdAsync(goalId)).ReturnsAsync(goal);

        // Act
        var result = await _controller.GetById(goalId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<WorkoutGoalDto>>().Subject;
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        _mockService.Setup(s => s.GetByIdAsync(goalId)).ReturnsAsync((WorkoutGoalDto?)null);

        // Act
        var result = await _controller.GetById(goalId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithGoals()
    {
        // Arrange
        var goals = new List<WorkoutGoalDto>
        {
            new() { Id = Guid.NewGuid(), UserId = _testUserId, WeeklyMinutes = 150 },
            new() { Id = Guid.NewGuid(), UserId = _testUserId, WeeklyMinutes = 200 }
        };
        _mockService.Setup(s => s.GetAllByUserAsync(_testUserId)).ReturnsAsync(goals);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<WorkoutGoalDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetActive_WithActiveGoal_ReturnsOk()
    {
        // Arrange
        var goal = new WorkoutGoalDto { Id = Guid.NewGuid(), UserId = _testUserId, WeeklyMinutes = 150, IsActive = true };
        _mockService.Setup(s => s.GetActiveByUserAsync(_testUserId)).ReturnsAsync(goal);

        // Act
        var result = await _controller.GetActive();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<WorkoutGoalDto>>().Subject;
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetActive_WithNoActiveGoal_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetActiveByUserAsync(_testUserId)).ReturnsAsync((WorkoutGoalDto?)null);

        // Act
        var result = await _controller.GetActive();

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Deactivate_WithValidId_ReturnsOk()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var deactivatedGoal = new WorkoutGoalDto { Id = goalId, UserId = _testUserId, IsActive = false };
        _mockService.Setup(s => s.DeactivateAsync(goalId, _testUserId)).ReturnsAsync(deactivatedGoal);

        // Act
        var result = await _controller.Deactivate(goalId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<WorkoutGoalDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Deactivate_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        _mockService.Setup(s => s.DeactivateAsync(goalId, _testUserId))
            .ThrowsAsync(new KeyNotFoundException("目標不存在"));

        // Act
        var result = await _controller.Deactivate(goalId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}
