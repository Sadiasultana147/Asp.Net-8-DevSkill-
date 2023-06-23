using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Serilog;
using System.Reflection;
using Serilog.Events;

namespace ECommerce.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((ctx, lc) => lc
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(builder.Configuration));

            try
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                var migrationAssembly = Assembly.GetExecutingAssembly().FullName;

                builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
                builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
                {
                    //containerBuilder.RegisterModule(new ApplicationModule());
                    //containerBuilder.RegisterModule(new InfrastructureModule());
                    //containerBuilder.RegisterModule(new PersistenceModule(connectionString,
                    //    migrationAssembly));
                    containerBuilder.RegisterModule(new WebModule());
                });

                // Add services to the container.
                //builder.Services.AddDbContext<ApplicationDbContext>(options =>
                //    options.UseSqlServer(connectionString,
                //    (x) => x.MigrationsAssembly(migrationAssembly)));

                //builder.Services.AddDatabaseDeveloperPageExceptionFilter();


                // Add services to the container.
                builder.Services.AddControllersWithViews();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
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
                Log.Information("Application Starting...");

                app.Run();
            }

            catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to start application.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}