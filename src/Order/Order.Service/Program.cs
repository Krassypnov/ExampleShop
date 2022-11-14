using Microsoft.AspNetCore.Authentication.Cookies;
using Order.Core.Abstraction;
using Order.Core.Service;
using Microsoft.EntityFrameworkCore;
using Order.DataAccess;
using Order.DataAccess.Abstraction;
using Order.DataAccess.Repo;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    { 
      Title = "Order Api",
      Version = "v1"
    });
    var filePath = Path.Combine(AppContext.BaseDirectory, "OrderApi.xml");
    c.IncludeXmlComments(filePath);
});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddHttpClient("CatalogService", c =>
    {
        c.BaseAddress = new Uri("https://localhost:7220/");
    });
builder.Services.AddHttpClient("DeliveryService", c =>
{
    c.BaseAddress = new Uri("https://localhost:7038/");
});

var dbPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Order.db";
builder.Services.AddDbContext<OrderContext>(options =>
                             options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IOrderService, OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
