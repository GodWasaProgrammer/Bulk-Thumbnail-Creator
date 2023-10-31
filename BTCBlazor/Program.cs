using Bulk_Thumbnail_Creator;
using Services;
using Services.Interfaces;
using Bulk_Thumbnail_Creator.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;
using Microsoft.EntityFrameworkCore;

namespace BTC_Blazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Directory.CreateDirectory(Settings.OutputDir);
            Directory.CreateDirectory(Settings.YTDLOutPutDir);
            Directory.CreateDirectory(Settings.TextAddedDir);
            Directory.CreateDirectory("logs");
            var builder = WebApplication.CreateBuilder(args);
            // Add configuration settings
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Services.AddDefaultIdentity<IdentityUser>()
                            .AddEntityFrameworkStores<ApplicationDbContext>();
            // Add services
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<PicDataService>();
            builder.Services.AddSingleton<ILogService, LogService>();
            builder.Services.AddMudServices();

            var app = builder.Build();

            var serviceProvider = app.Services;
            var logService = serviceProvider.GetRequiredService<ILogService>();
            
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), $"{Settings.TextAddedDir}")),
                RequestPath = "/text added"

            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}