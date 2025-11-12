using FitnessTracker.Api.Middleware;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Core.Services;
using FitnessTracker.Infrastructure.Data;
using FitnessTracker.Infrastructure.ExternalServices;
using FitnessTracker.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 讀取資料庫 Port 環境變數
var postgresPort = builder.Configuration["POSTGRES_PORT"] ?? "5432";
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");

// 如果連線字串中沒有指定 Port，或需要覆蓋 Port，可以在這裡處理
// 這裡保持原本的連線字串，因為容器內部使用標準 Port 5432
// 環境變數 POSTGRES_PORT 主要用於 Docker Compose 的對外 Port mapping

// Add DbContext
builder.Services.AddDbContext<FitnessTrackerDbContext>(options =>
    options.UseNpgsql(
        defaultConnection,
        npgsqlOptions => npgsqlOptions.MigrationsAssembly("FitnessTracker.Infrastructure")
    ));

// Add HttpClient for external services
builder.Services.AddHttpClient();

// Add generic repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Add repositories
builder.Services.AddScoped<IExerciseTypeRepository, ExerciseTypeRepository>();
builder.Services.AddScoped<IWorkoutRecordRepository, WorkoutRecordRepository>();
builder.Services.AddScoped<IWorkoutSetRepository, WorkoutSetRepository>();
builder.Services.AddScoped<IWorkoutGoalRepository, WorkoutGoalRepository>();

// Add authentication services
builder.Services.AddScoped<ILineLoginService, LineLoginService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Add business services
builder.Services.AddScoped<ICalorieCalculationService, CalorieCalculationService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IWorkoutGoalService, WorkoutGoalService>();
builder.Services.AddScoped<IWorkoutRecordService, WorkoutRecordService>();
builder.Services.AddScoped<IWorkoutSetService, WorkoutSetService>();
builder.Services.AddScoped<ExerciseTypeService>();
builder.Services.AddScoped<EquipmentService>();

// Add JWT authentication
var jwtKey = builder.Configuration["Jwt:SecretKey"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT:SecretKey configuration is missing");
}

var key = System.Text.Encoding.UTF8.GetBytes(jwtKey);

// Configure authentication
var authBuilder = builder.Services.AddAuthentication("Bearer");

// 在開發環境中，添加 Mock 認證支援
if (builder.Environment.IsDevelopment())
{
    authBuilder.AddScheme<AuthenticationSchemeOptions, FitnessTracker.Api.Middleware.MockAuthenticationHandler>(
        "MockAuth", 
        options => { });
}

authBuilder.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 統一使用 camelCase 命名，與前端 TypeScript 保持一致
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
var corsOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(',')
    ?? new[] { "http://localhost:5173", "http://localhost:5174", "http://localhost:5175", "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Use global exception middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// 在開發環境中，添加 Mock 認證的中介軟體
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.Contains("mock-jwt-token-"))
        {
            // 使用 Mock 認證
            var result = await context.AuthenticateAsync("MockAuth");
            if (result.Succeeded)
            {
                context.User = result.Principal;
            }
        }
        await next();
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply database migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FitnessTrackerDbContext>();
        context.Database.Migrate();
        
        // 在開發環境中，創建 Mock 使用者
        if (app.Environment.IsDevelopment())
        {
            var mockUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            if (!context.Users.Any(u => u.Id == mockUserId))
            {
                context.Users.Add(new FitnessTracker.Core.Entities.User
                {
                    Id = mockUserId,
                    LineUserId = "U1234567890abcdef",
                    DisplayName = "測試使用者",
                    PictureUrl = "https://cdn.vuetifyjs.com/images/avatars/1.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                context.SaveChanges();
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();
