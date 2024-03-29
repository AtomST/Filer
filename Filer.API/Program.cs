using Filer.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Filer.DAL;
using Filer.BL.Services;
using Filer.Services;

namespace Filer.API
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
            builder.Services.AddAuthentication("UData").AddCookie("UData", opt =>
            {
                opt.ExpireTimeSpan = TimeSpan.FromDays(30);
            });
            builder.Services.AddSingleton<UserRepository>();
            builder.Services.AddSingleton<FileRepository>();
            builder.Services.AddSingleton<FolderRepository>();
            builder.Services.AddSingleton<DbTransaction>();

            builder.Services.AddSingleton<FilerService>();
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