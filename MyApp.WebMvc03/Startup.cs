using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyApp.Admin.Security.Domains;
using MyApp.Admin.Security.Public.Data;
using MyApp.School.Public.Data;
using System;

namespace MyApp.WebMvc03
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            var defaultConnStr = Configuration.GetConnectionString("DefaultConnection");

            //This registers both DbContext. Each MUST have a unique MigrationsHistoryTable for Migrations to work
            services.AddDbContext<SecurityDbContext>(options =>
            {
                options.UseSqlServer(defaultConnStr, 
                    dbOptions => dbOptions.MigrationsHistoryTable("_EFMigrationHistory_Security"));

                if (_env.IsDevelopment())
                    options.EnableSensitiveDataLogging();
            });

            services.AddDbContext<SchoolDbContext>(options =>
            {
                options.UseSqlServer(defaultConnStr, 
                    dbOptions => dbOptions.MigrationsHistoryTable("_EFMigrationHistory_School"));

                if (_env.IsDevelopment())
                    options.EnableSensitiveDataLogging();
            });

            // must add this after scaffold Identity
            services.AddIdentity<UserProfile, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<SecurityDbContext>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();

            if (_env.IsDevelopment())
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddDistributedSqlServerCache(options =>
                {
                    options.ConnectionString =
                        Configuration.GetConnectionString("ConnStr_DistCache");
                    options.SchemaName = "dbo";
                    options.TableName = "AppCache";
                });
            }

            services.AddSession(options =>
            {
                options.Cookie.Name = ".MyApp.WebMvc03.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(30 * 60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                
                // must add this after scaffold Identity
                endpoints.MapRazorPages();
            });
        }
    }
}
