using BulkThumbnailCreator;
using BulkThumbnailCreator.Diagnostics;
using BulkThumbnailCreator.Interfaces;
using BulkThumbnailCreator.Services;
using Microsoft.AspNetCore.Components.Authorization;
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
                googleOptions.CallbackPath = new PathString("/signin-google");
            });

        // Add services
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
        builder.Services.AddSingleton<LogoService>();
        //builder.Services.AddScoped<JobService>();
        builder.Services.AddSingleton<JobReportService>();
        builder.Services.AddSingleton<IPerformanceTracker, PerformanceTracker>();
        builder.Services.AddScoped<IJobService>(provider =>
        {
            var inner = new JobService();
            var tracker = provider.GetRequiredService<IPerformanceTracker>();
            return JobServiceDecoratorFactory.Create(inner, tracker);
        });
        builder.Services.AddScoped<ILogService, LogService>();
        builder.Services.AddScoped<Settings>();
        //builder.Services.AddSingleton<UserStateService>();
        builder.Services.AddSingleton<IUserStateService>(provider =>
        {
            var inner = new UserStateServiceInstance();
            var tracker = provider.GetRequiredService<IPerformanceTracker>();
            return new TimedUserStateService(inner, tracker);
        });

        //builder.Services.AddScoped<Creator>();

        builder.Services.AddSingleton<JobReportService>();

        builder.Services.AddScoped<IProduction, Production>();
        builder.Services.AddScoped<ICreator>(provider =>
        {
            // Skapa original Creator med sina egna beroenden
            var logger = provider.GetRequiredService<ILogService>();
            var jobrepservice = provider.GetRequiredService<JobReportService>();
            var innerCreator = new Creator(logger); // _production skapas internt här

            // Skapa TimedCreator med alla dess beroenden
            var tracker = provider.GetRequiredService<IPerformanceTracker>();
            var decoratorLogger = provider.GetRequiredService<ILogger<TimedCreator>>();

            return new TimedCreator(
                innerCreator,
                tracker,
                decoratorLogger,
                jobrepservice,
                logger); // Skicka med ILogService för att skapa TimedProduction
        });
        builder.Services.AddMudServices();
        var app = builder.Build();

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
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        app.Run();
    }
}
