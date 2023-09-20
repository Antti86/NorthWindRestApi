using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NorthWindRestApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddSingleton<NorthwindOriginalContext>();

//builder.Services.AddTransient<NorthwindOriginalContext>();

builder.Services.AddDbContext<NorthwindOriginalContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Paikallinen")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("SalliKaikki",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("SalliKaikki");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
