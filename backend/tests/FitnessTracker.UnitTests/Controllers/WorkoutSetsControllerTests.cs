using FitnessTracker.Api.Controllers;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.Common;
using FitnessTracker.Shared.Dtos.WorkoutSets;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FitnessTracker.UnitTests.Controllers;

public class WorkoutSetsControllerTests
{
    private readonly Mock<IWorkoutSetService> _mockService;
    private readonly WorkoutSetsController _controller;
    private readonly Guid _testUserId = Guid.NewGuid();

    public WorkoutSetsControllerTests()
    {
        _mockService = new Mock<IWorkoutSetService>();
        _controller = new WorkoutSetsController(_mockService.Object);
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
        var createDto = new CreateWorkoutSetDto { WorkoutRecordId = 1, Repetitions = 10, Weight = 50 };
        var createdSet = new WorkoutSetDto 
        { 
            Id = 1, 
            WorkoutRecordId = 1, 
            SetNumber = 1,
            Repetitions = 10, 
            Weight = 50 
        };
        _mockService.Setup(s => s.CreateAsync(createDto, _testUserId)).ReturnsAsync(createdSet);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<WorkoutSetDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Repetitions.Should().Be(10);
    }

    [Fact]
    public async Task Create_WithUnauthorizedAccess_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateWorkoutSetDto { WorkoutRecordId = 1, Repetitions = 10 };
        _mockService.Setup(s => s.CreateAsync(createDto, _testUserId))
            .ThrowsAsync(new UnauthorizedAccessException("無權限"));

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Update_WithValidId_ReturnsOk()
    {
        // Arrange
        var updateDto = new UpdateWorkoutSetDto { Repetitions = 12, Weight = 55 };
        var updatedSet = new WorkoutSetDto 
        { 
            Id = 1, 
            WorkoutRecordId = 1, 
            SetNumber = 1,
            Repetitions = 12, 
            Weight = 55 
        };
        _mockService.Setup(s => s.UpdateAsync(1, updateDto, _testUserId)).ReturnsAsync(updatedSet);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<WorkoutSetDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Repetitions.Should().Be(12);
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateWorkoutSetDto { Repetitions = 12 };
        _mockService.Setup(s => s.UpdateAsync(999, updateDto, _testUserId))
            .ThrowsAsync(new KeyNotFoundException("訓練組數不存在"));

        // Act
        var result = await _controller.Update(999, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Update_WithUnauthorizedAccess_ReturnsUnauthorized()
    {
        // Arrange
        var updateDto = new UpdateWorkoutSetDto { Repetitions = 12 };
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
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(999, _testUserId))
            .ThrowsAsync(new KeyNotFoundException("訓練組數不存在"));

        // Act
        var result = await _controller.Delete(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_WithUnauthorizedAccess_ReturnsUnauthorized()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(1, _testUserId))
            .ThrowsAsync(new UnauthorizedAccessException("無權限"));

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetByWorkoutRecord_ReturnsOkWithSets()
    {
        // Arrange
        var sets = new List<WorkoutSetDto>
        {
            new() { Id = 1, WorkoutRecordId = 1, SetNumber = 1, Repetitions = 10 },
            new() { Id = 2, WorkoutRecordId = 1, SetNumber = 2, Repetitions = 8 }
        };
        _mockService.Setup(s => s.GetByWorkoutRecordAsync(1, _testUserId)).ReturnsAsync(sets);

        // Act
        var result = await _controller.GetByWorkoutRecord(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<WorkoutSetDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByWorkoutRecord_WithUnauthorizedAccess_ReturnsUnauthorized()
    {
        // Arrange
        _mockService.Setup(s => s.GetByWorkoutRecordAsync(1, _testUserId))
            .ThrowsAsync(new UnauthorizedAccessException("無權限"));

        // Act
        var result = await _controller.GetByWorkoutRecord(1);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetLastSet_WithExistingData_ReturnsOk()
    {
        // Arrange
        var lastSet = new WorkoutSetDto { Id = 5, WorkoutRecordId = 1, SetNumber = 3, Repetitions = 8, Weight = 60 };
        _mockService.Setup(s => s.GetLastSetAsync(1, _testUserId)).ReturnsAsync(lastSet);

        // Act
        var result = await _controller.GetLastSet(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<WorkoutSetDto?>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Repetitions.Should().Be(8);
    }

    [Fact]
    public async Task GetLastSet_WithNoData_ReturnsOkWithNull()
    {
        // Arrange
        _mockService.Setup(s => s.GetLastSetAsync(1, _testUserId)).ReturnsAsync((WorkoutSetDto?)null);

        // Act
        var result = await _controller.GetLastSet(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<WorkoutSetDto?>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().BeNull();
    }
}
