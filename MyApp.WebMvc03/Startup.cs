using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyApp.Admin.Security.Domains;
using MyApp.Admin.Security.Public.Data;
using MyApp.Admin.Security.Public.Services;
using MyApp.School.Public.Services;
using MyApp.WebMvc03.Data;
using NetCore.AutoRegisterDi;
using System;
using System.Reflection;

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

            // Extension to register all the databases context used in this app
            services.RegisterDatabases(Configuration, _env);

            // must add this after scaffold Identity
            services.AddIdentity<UserProfile, CustomRole>(options => options.SignIn.RequireConfirmedAccount = true)
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

            //This registers all the services across all the projects in this application
            var diLogs = services.RegisterAssemblyPublicNonGenericClasses(
                    Assembly.GetAssembly(typeof(ICacheControlService)),
                    Assembly.GetAssembly(typeof(IRoleService)),
                    Assembly.GetAssembly(typeof(IUserRoleService)),
                    Assembly.GetAssembly(typeof(ICourseService)),
                    Assembly.GetAssembly(typeof(IDepartmentService))
                )
                .AsPublicImplementedInterfaces();

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
