using Delivery.Core.Abstraction;
using Delivery.Core.Service;
using Delivery.DataAccess;
using Delivery.DataAccess.Abstraction;
using Delivery.DataAccess.Repo;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("CatalogService", c =>
{
    c.BaseAddress = new Uri("https://localhost:7220/");
});
builder.Services.AddHttpClient("OrderService", c =>
{
    c.BaseAddress = new Uri("https://localhost:7116/");
});

var dbPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Delivery.db";
builder.Services.AddDbContext<DeliveryContext>(options =>
                             options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddTransient<IDeliveryService, DeliveryService>();

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
