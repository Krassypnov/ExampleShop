using Catalog.DataAccess;
using Catalog.Core;
using Microsoft.EntityFrameworkCore;
using Catalog.DataAccess.Abstraction;
using Catalog.DataAccess.Repo;
using Catalog.Core.Abstraction;
using Catalog.Core.Service;
using Microsoft.OpenApi.Models;

namespace Catalog.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Catalog Api",
                    Version = "v1"
                });
                var filePath = Path.Combine(AppContext.BaseDirectory, "CatalogApi.xml");
                c.IncludeXmlComments(filePath);
            });

            var dbPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Catalog.db";
            builder.Services.AddDbContext<CatalogContext>(options =>
                             options.UseSqlite($"Data Source={dbPath}"));

            builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
            builder.Services.AddScoped<IReserveRepository, ReserveRepository>();
            builder.Services.AddTransient<ICatalogService, CatalogService>();
            builder.Services.AddTransient<IReservationService, ReservationService>();

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
        }
    }
}
