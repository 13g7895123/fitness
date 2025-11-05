using FitnessTracker.Api.Middleware;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Core.Services;
using FitnessTracker.Infrastructure.Data;
using FitnessTracker.Infrastructure.ExternalServices;
using FitnessTracker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<FitnessTrackerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.MigrationsAssembly("FitnessTracker.Infrastructure")
    ));

// Add HttpClient for external services
builder.Services.AddHttpClient();

// Add generic repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Add repositories
builder.Services.AddScoped<IExerciseTypeRepository, ExerciseTypeRepository>();
builder.Services.AddScoped<IWorkoutRecordRepository, WorkoutRecordRepository>();
builder.Services.AddScoped<IWorkoutGoalRepository, WorkoutGoalRepository>();

// Add authentication services
builder.Services.AddScoped<ILineLoginService, LineLoginService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Add business services
builder.Services.AddScoped<ICalorieCalculationService, CalorieCalculationService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IWorkoutRecordService, WorkoutRecordService>();
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

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5174", "http://localhost:3000")
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
