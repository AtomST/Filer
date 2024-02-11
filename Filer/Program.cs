using Filer.DAL;
using Filer.DAL.Repositories;
using Filer.DAL.Repository;
using Filer.Service;
using Microsoft.EntityFrameworkCore;

namespace Filer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<FilerDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("FilerDbConnection"));
            });
            builder.Services.AddAuthentication("UData").AddCookie("UData",opt =>
            {
                opt.ExpireTimeSpan = TimeSpan.FromDays(30);
            });
            builder.Services.AddTransient<UserRepository>();
            builder.Services.AddTransient<FileRepository>();
            builder.Services.AddTransient<FolderRepository>();


            builder.Services.AddTransient<FilerService>();
            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute("default", "filer/{folder:long}", new { Controller = "Filer", action = "Main" });
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Filer}/{action=main}");

            app.Run();
        }
    }
}