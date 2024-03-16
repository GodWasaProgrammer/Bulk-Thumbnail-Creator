// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BulkThumbnailCreator;
using BulkThumbnailCreator.Interfaces;
using BulkThumbnailCreator.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;
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
        builder.Services.AddScoped<CreatorService>();
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
