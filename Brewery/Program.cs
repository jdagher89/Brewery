

using Brewery.Context;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = new ConfigurationBuilder()
   .AddJsonFile("appsettings.json", true, true)
   .Build();
var BreweryConnectionString = configuration.GetConnectionString("BreweryConn");


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BreweryDb>(options => options.UseSqlServer(BreweryConnectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
