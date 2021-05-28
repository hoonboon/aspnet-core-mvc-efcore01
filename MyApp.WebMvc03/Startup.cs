using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyApp.Admin.Security.Domains;
using MyApp.Admin.Security.Public.Constants;
using MyApp.Admin.Security.Public.Data;
using MyApp.Admin.Security.Public.Extensions;
using MyApp.Admin.Security.Public.PermissionControl.Policy;
using MyApp.Admin.Security.Public.Services;
using MyApp.School.Public.Services;
using MyApp.WebMvc03.Data;
using NetCore.AutoRegisterDi;
using System;
using System.Linq;
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false; // true;
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

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
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            services.AddControllersWithViews();
            services.AddRazorPages();

            //Register the Permission policy handlers
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            ////This is needed to implement the data authorize code 
            //services.AddScoped<IGetClaimsProvider, GetClaimsFromUser>();

            //This registers all the services across all the projects in this application
            var diLogs = services.RegisterAssemblyPublicNonGenericClasses(
                    Assembly.GetAssembly(typeof(ICacheControlService)),
                    //Assembly.GetAssembly(typeof(IRoleService)),
                    //Assembly.GetAssembly(typeof(IUserRoleService)),
                    Assembly.GetAssembly(typeof(ICourseService))
                //Assembly.GetAssembly(typeof(IDepartmentService))
                )
                .Where(c => c.Name.EndsWith("Service"))
                .AsPublicImplementedInterfaces();
            //services.UserImpersonationRegister();

            //This enables Cookies for authentication and adds the feature and data claims to the user
            services.ConfigureCookiesForExtraAuth();

            ////This has to come after the ConfigureCookiesForExtraAuth settings, which sets up the IAuthChanges
            //services.ConfigureGenericServicesEntities(typeof(ExtraAuthorizeDbContext), typeof(CompanyDbContext))
            //    .ScanAssemblesForDtos(Assembly.GetAssembly(typeof(ListUsersDto)))
            //    .RegisterGenericServices();

            //services.AddAuthorization(options =>
            //{
            //    options.FallbackPolicy = new AuthorizationPolicyBuilder()
            //        .RequireAuthenticatedUser()
            //        .Build();
            //});



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

            app.UseCookiePolicy();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            //This should come AFTER the app.UseAuthentication() call
            //If UpdateCookieOnChange this adds a header which has the time that the user's claims were updated
            //thanks to https://stackoverflow.com/a/48610119/1434764
            app.Use((context, next) =>
            {
                var lastTimeUserPermissionsSet = context.User.Claims
                    .SingleOrDefault(x => x.Type == PermissionConstants.LastPermissionsUpdatedClaimType)?.Value;
                if (lastTimeUserPermissionsSet != null)
                    context.Response.Headers["Last-Time-Users-Permissions-Updated"] = lastTimeUserPermissionsSet;
                return next.Invoke();
            });

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
