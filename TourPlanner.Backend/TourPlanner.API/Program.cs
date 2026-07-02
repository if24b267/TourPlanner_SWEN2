using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using TourPlanner.BL.Configuration;
using TourPlanner.BL.Interfaces;
using TourPlanner.BL.Services;
using TourPlanner.DAL;

// Bootstrap-Logger faengt auch Fehler beim Start selbst ab
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Config-Binding (kommt aus appsettings.json / appsettings.Development.json)
builder.Services.Configure<StorageOptions>(
    builder.Configuration.GetSection("Storage"));

builder.Services.Configure<OpenRouteServiceOptions>(
    builder.Configuration.GetSection("OpenRouteService"));

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
    };
});

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient<IRouteService, OpenRouteService>();

// DbContext
builder.Services.AddDbContext<TourDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TourPlannerDb")));

// Business Layer
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<ITourLogService, TourLogService>();
builder.Services.AddScoped<IImportExportService, ImportExportService>();
builder.Services.AddScoped<IAchievementService, AchievementService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header. Beispiel: \"Bearer {token}\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AngularDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();