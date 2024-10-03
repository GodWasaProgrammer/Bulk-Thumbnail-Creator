using System.Net;
using BulkThumbnailCreator;
using BulkThumbnailCreator.Interfaces;
using BulkThumbnailCreator.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;
using WebUI.Areas.Identity;
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
        builder.Services.AddSingleton<LogoService>();
        builder.Services.AddScoped<JobService>();
        builder.Services.AddScoped<ILogService, LogService>();
        builder.Services.AddScoped<Settings>();
        builder.Services.AddSingleton<UserStateService>();
        builder.Services.AddScoped<Creator>();
        builder.Services.AddMudServices();
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.KnownProxies.Add(IPAddress.Parse("172.19.0.3")); // nginx ip
            options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse("172.19.0.3"), 24));
        });
        var app = builder.Build();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            // app.UseHsts();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "TextAdded")),
            RequestPath = "/TextAdded"
        });
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "DankMemeStash")),
            RequestPath = "/DankMemeStash"
        });
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Fonts")),
            RequestPath = "/Fonts"
        });
        app.Use(async (context, next) =>
        {
            Console.WriteLine($"Current scheme:{context.Request.Scheme.ToString()}");
            context.Request.Scheme = "https";
            var proto = context.Request.Headers["X-Forwarded-Proto"].ToString();
            Console.WriteLine($"X-Forwarded-Proto: {proto}");
            Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");

            var location = context.Response.Headers["Location"].ToString();
            Console.WriteLine($"Original Redirect Location: {location}");
            await next.Invoke();
        });

        app.UseRouting();
        app.Use(async (context, next) =>
        {
            await next.Invoke();

            // Logga alla statuskoder för att se om vi når den här delen av pipelinen
            Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");

            var location = context.Response.Headers["Location"].ToString();
            Console.WriteLine($"Original Redirect Location: {location}");

            if (location.StartsWith("http://"))
            {
                // Logga till konsolen
                Console.WriteLine($"Redirect från HTTP till HTTPS: {location}");

                // Ändra Location-header till HTTPS
                context.Response.Headers["Location"] = location.Replace("http://", "https://");

                // Logga den nya redirecten till konsolen
                Console.WriteLine($"Uppdaterad redirect: {context.Response.Headers["Location"]}");
            }

        });

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        app.Run();
    }
}
