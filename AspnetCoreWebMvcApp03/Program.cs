using AspnetCoreWebMvcApp03.Areas.Identity.Data;
using AspnetCoreWebMvcApp03.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Threading.Tasks;

namespace AspnetCoreWebMvcApp03
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Debug("init main");

                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<SchoolContext>();

                        logger.Info("DbInitializer.Initialize() start");
                        DbInitializer.Initialize(context);
                        logger.Info("DbInitializer.Initialize() end");

                        var userManager = services.GetRequiredService<UserManager<UserProfile>>();
                        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                        logger.Info("DbInitializer.SeedRolesAsync() start");
                        await DbInitializer.SeedRolesAsync(userManager, roleManager);
                        logger.Info("DbInitializer.SeedRolesAsync() end");

                        var config = host.Services.GetRequiredService<IConfiguration>();
                        var defaultUserPwd = config["SeedData:DefaultUserPwd"];
                        logger.Info($"using defaultUserPwd={defaultUserPwd}");
                        logger.Info("DbInitializer.SeedSuperAdminAsync() start");
                        await DbInitializer.SeedSuperAdminAsync(userManager, defaultUserPwd);
                        logger.Info("DbInitializer.SeedSuperAdminAsync() end");

                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "An error occurred creating the DB.");
                    }
                }

                host.Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.AddServerHeader = false;
                    });

                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    // logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
    }
}
