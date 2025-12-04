using FitnessTracker.Api.Controllers;
using FitnessTracker.Core.Services;
using FitnessTracker.Shared.Dtos.Common;
using FitnessTracker.Shared.Dtos.Equipments;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FitnessTracker.UnitTests.Controllers;

public class EquipmentsControllerTests
{
    private readonly Mock<EquipmentService> _mockService;
    private readonly EquipmentsController _controller;
    private readonly Guid _testUserId = Guid.NewGuid();

    public EquipmentsControllerTests()
    {
        _mockService = new Mock<EquipmentService>(MockBehavior.Strict, null!);
        _controller = new EquipmentsController(_mockService.Object);
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
    public async Task GetAll_ReturnsOkWithEquipments()
    {
        // Arrange
        var equipments = new List<EquipmentDto>
        {
            new() { Id = 1, Name = "啞鈴" },
            new() { Id = 2, Name = "槓鈴" }
        };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(equipments);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<EquipmentDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkWithEquipment()
    {
        // Arrange
        var equipment = new EquipmentDto { Id = 1, Name = "啞鈴" };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(equipment);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<EquipmentDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Name.Should().Be("啞鈴");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((EquipmentDto?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = notFoundResult.Value.Should().BeOfType<ApiResponse<EquipmentDto>>().Subject;
        response.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Create_WithValidDto_ReturnsCreatedResult()
    {
        // Arrange
        var createDto = new CreateEquipmentDto { Name = "新器材" };
        var createdEquipment = new EquipmentDto { Id = 1, Name = "新器材" };
        _mockService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdEquipment);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<ApiResponse<EquipmentDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Update_WithValidId_ReturnsOk()
    {
        // Arrange
        var updateDto = new UpdateEquipmentDto { Name = "更新器材" };
        var updatedEquipment = new EquipmentDto { Id = 1, Name = "更新器材" };
        _mockService.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync(updatedEquipment);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<EquipmentDto>>().Subject;
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateEquipmentDto { Name = "更新器材" };
        _mockService.Setup(s => s.UpdateAsync(999, updateDto)).ReturnsAsync((EquipmentDto?)null);

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
