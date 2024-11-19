using System.Configuration;
using System.Net;
using App.Models;
using App.Services;
using Microsoft.EntityFrameworkCore;

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var sqlPort = Environment.GetEnvironmentVariable("MSSQL_PORT");
            var sqlPassword = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");

            var connectionString = "Server=sqlserverdocker," +sqlPort +";Database=master;User Id=sa;Password=" +sqlPassword + ";Encrypt=false;TrustServerCertificate=true;MultipleActiveResultSets=true;";

            builder.Services.AddDbContext<SqlContext>(
            options =>
            {
                options.UseSqlServer(connectionString);

            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<MinioService>();
            builder.Services.AddScoped<ArchivosService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Archivos}/{action=Archivos}/{id?}");

            app.Run();
        }
    }
}
