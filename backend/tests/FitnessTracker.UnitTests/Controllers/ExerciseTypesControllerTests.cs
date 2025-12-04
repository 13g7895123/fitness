using FitnessTracker.Api.Controllers;
using FitnessTracker.Core.Services;
using FitnessTracker.Shared.Dtos.Common;
using FitnessTracker.Shared.Dtos.ExerciseTypes;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FitnessTracker.UnitTests.Controllers;

public class ExerciseTypesControllerTests
{
    private readonly Mock<ExerciseTypeService> _mockService;
    private readonly ExerciseTypesController _controller;
    private readonly Guid _testUserId = Guid.NewGuid();

    public ExerciseTypesControllerTests()
    {
        _mockService = new Mock<ExerciseTypeService>(MockBehavior.Strict, null!);
        _controller = new ExerciseTypesController(_mockService.Object);
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
    public async Task GetAll_ReturnsOkWithExerciseTypes()
    {
        // Arrange
        var exerciseTypes = new List<ExerciseTypeDto>
        {
            new() { Id = 1, Name = "跑步", DefaultMET = 8.0m },
            new() { Id = 2, Name = "游泳", DefaultMET = 7.0m }
        };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(exerciseTypes);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<ExerciseTypeDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkWithExerciseType()
    {
        // Arrange
        var exerciseType = new ExerciseTypeDto { Id = 1, Name = "跑步", DefaultMET = 8.0m };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(exerciseType);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<ExerciseTypeDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Name.Should().Be("跑步");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((ExerciseTypeDto?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = notFoundResult.Value.Should().BeOfType<ApiResponse<ExerciseTypeDto>>().Subject;
        response.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Search_ReturnsOkWithMatchingExerciseTypes()
    {
        // Arrange
        var exerciseTypes = new List<ExerciseTypeDto>
        {
            new() { Id = 1, Name = "跑步", DefaultMET = 8.0m }
        };
        _mockService.Setup(s => s.SearchAsync("跑")).ReturnsAsync(exerciseTypes);

        // Act
        var result = await _controller.Search("跑");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<ExerciseTypeDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task Create_WithValidDto_ReturnsCreatedResult()
    {
        // Arrange
        var createDto = new CreateExerciseTypeDto { Name = "新運動", DefaultMET = 5.0m };
        var createdExerciseType = new ExerciseTypeDto { Id = 1, Name = "新運動", DefaultMET = 5.0m };
        
        _mockService.Setup(s => s.IsNameExistsAsync(createDto.Name, null)).ReturnsAsync(false);
        _mockService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdExerciseType);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<ApiResponse<ExerciseTypeDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Create_WithDuplicateName_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateExerciseTypeDto { Name = "已存在運動", DefaultMET = 5.0m };
        _mockService.Setup(s => s.IsNameExistsAsync(createDto.Name, null)).ReturnsAsync(true);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var response = badRequestResult.Value.Should().BeOfType<ApiResponse<ExerciseTypeDto>>().Subject;
        response.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Update_WithValidId_ReturnsOk()
    {
        // Arrange
        var updateDto = new UpdateExerciseTypeDto { Name = "更新運動", DefaultMET = 6.0m };
        var updatedExerciseType = new ExerciseTypeDto { Id = 1, Name = "更新運動", DefaultMET = 6.0m };
        _mockService.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync(updatedExerciseType);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<ExerciseTypeDto>>().Subject;
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateExerciseTypeDto { Name = "更新運動", DefaultMET = 6.0m };
        _mockService.Setup(s => s.UpdateAsync(999, updateDto)).ReturnsAsync((ExerciseTypeDto?)null);

        // Act
        var result = await _controller.Update(999, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_WithValidId_ReturnsOk()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        response.Success.Should().BeTrue();
    }
}
