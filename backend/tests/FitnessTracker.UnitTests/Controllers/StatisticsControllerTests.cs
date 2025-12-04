using FitnessTracker.Api.Controllers;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.Common;
using FitnessTracker.Shared.Dtos.Statistics;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FitnessTracker.UnitTests.Controllers;

public class StatisticsControllerTests
{
    private readonly Mock<IStatisticsService> _mockService;
    private readonly StatisticsController _controller;
    private readonly Guid _testUserId = Guid.NewGuid();

    public StatisticsControllerTests()
    {
        _mockService = new Mock<IStatisticsService>();
        _controller = new StatisticsController(_mockService.Object);
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
    public async Task GetWeeklySummary_ReturnsOkWithData()
    {
        // Arrange
        var summary = new WeeklySummaryDto
        {
            WeekStartDate = DateTime.Today.AddDays(-7),
            WeekEndDate = DateTime.Today,
            TotalDurationMinutes = 300,
            TotalCaloriesBurned = 1500,
            WorkoutDays = 5
        };
        _mockService.Setup(s => s.GetWeeklySummaryAsync(_testUserId, null)).ReturnsAsync(summary);

        // Act
        var result = await _controller.GetWeeklySummary(null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<WeeklySummaryDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.TotalDurationMinutes.Should().Be(300);
    }

    [Fact]
    public async Task GetWeeklySummary_WithDate_ReturnsOkWithData()
    {
        // Arrange
        var targetDate = new DateTime(2024, 1, 15);
        var summary = new WeeklySummaryDto
        {
            WeekStartDate = new DateTime(2024, 1, 14),
            WeekEndDate = new DateTime(2024, 1, 20),
            TotalDurationMinutes = 200
        };
        _mockService.Setup(s => s.GetWeeklySummaryAsync(_testUserId, targetDate)).ReturnsAsync(summary);

        // Act
        var result = await _controller.GetWeeklySummary(targetDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<WeeklySummaryDto>>().Subject;
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetMonthlySummary_ReturnsOkWithData()
    {
        // Arrange
        var summary = new MonthlySummaryDto
        {
            Month = "2024-01",
            TotalDurationMinutes = 1200,
            TotalCaloriesBurned = 6000,
            WorkoutDays = 20
        };
        _mockService.Setup(s => s.GetMonthlySummaryAsync(_testUserId, null, null)).ReturnsAsync(summary);

        // Act
        var result = await _controller.GetMonthlySummary(null, null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<MonthlySummaryDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.TotalDurationMinutes.Should().Be(1200);
    }

    [Fact]
    public async Task GetMonthlySummary_WithYearAndMonth_ReturnsOkWithData()
    {
        // Arrange
        var summary = new MonthlySummaryDto
        {
            Month = "2024-01",
            TotalDurationMinutes = 1000
        };
        _mockService.Setup(s => s.GetMonthlySummaryAsync(_testUserId, 2024, 1)).ReturnsAsync(summary);

        // Act
        var result = await _controller.GetMonthlySummary(2024, 1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<MonthlySummaryDto>>().Subject;
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetDailyBreakdown_ReturnsOkWithData()
    {
        // Arrange
        var targetDate = new DateTime(2024, 1, 15);
        var dailyData = new TrendDataDto
        {
            Date = "2024-01-15",
            DurationMinutes = 60,
            CaloriesBurned = 300
        };
        _mockService.Setup(s => s.GetDailyBreakdownAsync(_testUserId, targetDate)).ReturnsAsync(dailyData);

        // Act
        var result = await _controller.GetDailyBreakdown(targetDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<TrendDataDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.DurationMinutes.Should().Be(60);
    }

    [Fact]
    public async Task GetTrends_WithDefaultPeriod_ReturnsOkWithData()
    {
        // Arrange
        var trends = new List<TrendDataDto>
        {
            new() { Date = "2024-01-14", DurationMinutes = 50 },
            new() { Date = "2024-01-15", DurationMinutes = 60 }
        };
        _mockService.Setup(s => s.GetTrendsAsync(_testUserId, "day")).ReturnsAsync(trends);

        // Act
        var result = await _controller.GetTrends("day");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<TrendDataDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTrends_WithWeekPeriod_ReturnsOkWithData()
    {
        // Arrange
        var trends = new List<TrendDataDto>
        {
            new() { Date = "2024-W01", DurationMinutes = 300, PeriodType = "week" }
        };
        _mockService.Setup(s => s.GetTrendsAsync(_testUserId, "week")).ReturnsAsync(trends);

        // Act
        var result = await _controller.GetTrends("week");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<TrendDataDto>>>().Subject;
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetExerciseDistribution_ReturnsOkWithData()
    {
        // Arrange
        var distribution = new List<ExerciseDistributionDto>
        {
            new() { ExerciseName = "跑步", TotalDurationMinutes = 200, PercentageOfTotal = 50 },
            new() { ExerciseName = "游泳", TotalDurationMinutes = 200, PercentageOfTotal = 50 }
        };
        _mockService.Setup(s => s.GetExerciseDistributionAsync(_testUserId)).ReturnsAsync(distribution);

        // Act
        var result = await _controller.GetExerciseDistribution();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<ExerciseDistributionDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
    }
}
