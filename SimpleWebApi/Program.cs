// Cách fix 1: Tạo Public Program Class cho Testing (build suceeded!)
using Microsoft.EntityFrameworkCore;
using SimpleWebApi.Core.Interfaces;
using SimpleWebApi.Core.Services;
using SimpleWebApi.Data;
using SimpleWebApi.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("SimpleWebApiDb"));

// Dependency Injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.Run();

// Make Program class public for testing
public partial class Program { }





// phần integration của unitest đang có lỗi error CS0122: 'Program' is inaccessible due to its protection level 
// Explanation: Lỗi này xảy ra vì class Program trong .NET 6+ được tạo tự động với protection level internal
// using Microsoft.EntityFrameworkCore;
// using SimpleWebApi.Core.Interfaces;
// using SimpleWebApi.Core.Services;
// using SimpleWebApi.Data;
// using SimpleWebApi.Data.Repositories;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container
// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// // Database
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseInMemoryDatabase("SimpleWebApiDb"));

// // Dependency Injection
// builder.Services.AddScoped<IProductRepository, ProductRepository>();
// builder.Services.AddScoped<IProductService, ProductService>();

// var app = builder.Build();

// // Configure the HTTP request pipeline
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();
// app.UseAuthorization();
// app.MapControllers();

// // Seed database
// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     context.Database.EnsureCreated();
// }

// app.Run();
