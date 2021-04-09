using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyApp.Admin.Security.Public.Data;
using MyApp.School.Public.Data;

namespace MyApp.WebMvc03.Data
{
    public static class AddDatabasesExtension
    {
        public static void RegisterDatabases(
            this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            var defaultConnStr = configuration.GetConnectionString("DefaultConnection");

            //This registers both DbContext. Each MUST have a unique MigrationsHistoryTable for Migrations to work
            services.AddDbContext<SecurityDbContext>(options =>
            {
                options.UseSqlServer(defaultConnStr,
                    dbOptions => dbOptions.MigrationsHistoryTable("_EFMigrationHistory_Security"));

                if (env.IsDevelopment())
                    options.EnableSensitiveDataLogging();
            });

            services.AddDbContext<SchoolDbContext>(options =>
            {
                options.UseSqlServer(defaultConnStr,
                    dbOptions => dbOptions.MigrationsHistoryTable("_EFMigrationHistory_School"));

                if (env.IsDevelopment())
                    options.EnableSensitiveDataLogging();
            });


        }
    }
}
