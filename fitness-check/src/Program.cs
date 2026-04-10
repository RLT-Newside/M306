using FitnessCheck.Data;
using Microsoft.EntityFrameworkCore;
using FitnessCheck.Services;
using Scalar.AspNetCore;
using FitnessCheck.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add OpenAPI/Swagger UI using built-in .NET 9 support
builder.Services.AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });

// Add In-Memory caching
builder.Services.AddSingleton<GenderMemoryCache>();
builder.Services.AddSingleton<CohortMemoryCache>();

// Add JWT authorization
builder.Services.AddAuthentication().AddJwtBearer();

// Establish db connection
var connectionString = builder.Configuration["ConnectionStrings:MariaDb"];
var serverVersion = new MySqlServerVersion(new Version(8, 0, 26));
builder.Services.AddDbContextPool<FitnessCheckDbContext>(options => options.UseMySql(connectionString, serverVersion));

// Registering services
builder.Services.AddAutoMapper(configuration => { configuration.AddProfile<FitnessCheckMappingProfile>(); });
builder.Services.AddScoped<DeletionDeadlineResolver>();
builder.Services.AddScoped<IPointsCalculator, PointsCalculator>();
builder.Services.AddScoped<IGenderService, GoogleCloudFunctionsGenderService>();
builder.Services.AddScoped<ICohortService, GoogleCloudFunctionsCohortService>();
builder.Services.AddScoped<IBestAttemptService, BestAttemptService>();
builder.Services.AddScoped<IAttemptService, AttemptService>();

// Configuration / Options
builder.Services.Configure<MaxAllowedAttemptsOptions>(builder.Configuration.GetSection(MaxAllowedAttemptsOptions.ConfigurationPath));
builder.Services.Configure<GoogleCloudFunctionOptions>(builder.Configuration.GetSection(GoogleCloudFunctionOptions.ConfigurationPath));
builder.Services.Configure<DeletionDeadlineOptions>(builder.Configuration.GetSection(DeletionDeadlineOptions.ConfigurationPath));
builder.Services.Configure<DisciplineConfigurationOptions>(builder.Configuration.GetSection(DisciplineConfigurationOptions.ConfigurationPath));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/api-doc");
}

// Enable CORS for development
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
