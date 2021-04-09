using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyApp.Admin.Security.Domains;
using MyApp.Admin.Security.Public.Data;
using MyApp.School.Public.Data;
using System;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Data
{
    public static class MainDatabaseSeederExtension
    {
        public static async Task<IHost> SetupDatabaseAsync(this IHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var env = services.GetRequiredService<IWebHostEnvironment>();
                var logger = services.GetRequiredService<ILogger<Program>>();

                var userManager = services.GetRequiredService<UserManager<UserProfile>>();
                var roleManager = services.GetRequiredService<RoleManager<CustomRole>>();
                
                var config = webHost.Services.GetRequiredService<IConfiguration>();
                var defaultUserPwd = config["SeedData:DefaultUserPwd"];
                logger.LogInformation($"using defaultUserPwd={defaultUserPwd}");

                var securityContext = services.GetRequiredService<SecurityDbContext>();
                var schoolContext = services.GetRequiredService<SchoolDbContext>();
                try
                {
                    logger.LogInformation("securityContext.SeedDatabaseWithSecurityDataAsync starts");
                    await securityContext.SeedDatabaseWithSecurityDataAsync(
                        userManager, roleManager, defaultUserPwd);
                    logger.LogInformation("securityContext.SeedDatabaseWithSecurityDataAsync ends");

                    logger.LogInformation("schoolContext.SeedDatabaseWithSchoolDataAsync starts");
                    await schoolContext.SeedDatabaseWithSchoolDataAsync();
                    logger.LogInformation("schoolContext.SeedDatabaseWithSchoolDataAsync ends");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while creating/migrating/seeding the SQL database.");
                    throw;
                }
            }

            return webHost;
        }

    }
}
