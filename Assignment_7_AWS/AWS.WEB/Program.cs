using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AWS.Infrastructure;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(builder.Configuration));
try
{
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterModule(new AWSServiceModule(builder.Configuration));

    });


    // Add services to the container.
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages(); // Add this line to configure Razor Pages services

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }


    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages(); // Moved MapRazorPages to the end

    Log.Information("Application Starting...");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to start applications.");
}
finally
{
    Log.CloseAndFlush();
}

