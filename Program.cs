using AnimeApi.Data;
using AnimeApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MySQL connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IAnimeService, AnimeService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// ✅ Apply CORS policy here BEFORE UseAuthorization
app.UseCors("AllowLocalhost3000");

app.UseAuthorization();
app.MapControllers();
app.Run();
