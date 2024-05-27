using BulkThumbnailCreator;
using BulkThumbnailCreator.DataMethods;
using BulkThumbnailCreator.Interfaces;
using BulkThumbnailCreator.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Client;
using MudBlazor.Services;
using WebUI.Areas.Identity;
using WebUI.Components;
using WebUI.Pages;
namespace WebUI;

public static class Program
{
    public static void Main(string[] args)
    {
        Directory.CreateDirectory("output");
        Directory.CreateDirectory("YTDL");
        Directory.CreateDirectory("TextAdded");
        Directory.CreateDirectory("logs");
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;
        // Add configuration settings
        builder.Configuration.AddJsonFile("appsettings.json", optional: false);
        builder.Services.AddDefaultIdentity<IdentityUser>()
                        .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddAuthentication().AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = Environment.GetEnvironmentVariable("ClientId")?.Trim();
            googleOptions.ClientSecret = Environment.GetEnvironmentVariable("ClientSecret")?.Trim();
        });
        // Add services
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
        builder.Services.AddScoped<CreatorService>();
        builder.Services.AddSingleton<LogoService>();
        builder.Services.AddScoped<JobService>();
        builder.Services.AddScoped<ILogService, LogService>();
        builder.Services.AddScoped<Settings>();
        builder.Services.AddSingleton<UserStateService>();
        builder.Services.AddMudServices();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "TextAdded")),
            RequestPath = "/TextAdded"
        });
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        app.Run();
    }
}
