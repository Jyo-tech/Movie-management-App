using Microsoft.EntityFrameworkCore;
using MovieApp.API.ExceptionHandling;
using MovieApp.Application.Interfaces;
using MovieApp.Application.Services;
using MovieApp.Domain.Interfaces;
using MovieApp.Infrastructure.Data;
using MovieApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMovieRepository, MovieRepository>();

builder.Services.AddScoped<IMovieService, MovieService>();

var origins = builder.Configuration
    .GetSection("AllowAngularUI:Origins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularUI", policy =>
    {
        policy.WithOrigins(origins ?? Array.Empty<string>())
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("AllowAngularUI");

app.UseAuthorization();

app.MapControllers();

var seedEnabled = app.Configuration.GetValue<bool?>("Seed:Enabled") ?? app.Environment.IsDevelopment();
var forceFullReseed = app.Configuration.GetValue("Seed:ForceFullReseed", false) && app.Environment.IsDevelopment();

if (seedEnabled)
{
    using var scope = app.Services.CreateScope();
    var sp = scope.ServiceProvider;
    var db = sp.GetRequiredService<AppDbContext>();
    var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(DbSeeder));
    var contentRoot = sp.GetRequiredService<IHostEnvironment>().ContentRootPath;

    await DbSeeder.SeedAsync(db, logger, forceFullReseed, contentRoot);
}

app.Run();
