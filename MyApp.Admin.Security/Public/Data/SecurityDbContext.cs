using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApp.Admin.Security.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.Public.Data
{
    /**
     * Notes on EF Migrations:
     * 
     * Execute the following command in a Developer Command Prompt from the startup project directory: MyApp.WebMvc03
     * 
     * Sample Command: 
     * - Create Migration: ...\MyApp.WebMvc03>dotnet ef migrations add InitialCreate --context SecurityDbContext --project "../MyApp.Admin.Security/MyApp.Admin.Security.csproj"
     * - Update Database: ...\MyApp.WebMvc03>dotnet ef database update --context SecurityDbContext --project "../MyApp.Admin.Security/MyApp.Admin.Security.csproj"
     * 
     */

    // must extend IdentityDbContext first before being re-used to Scaffold Identity
    public class SecurityDbContext : IdentityDbContext<UserProfile, CustomRole, string>
    {
        public static readonly string DbSchemaName = "Security";

        private readonly ILogger<SecurityDbContext> _logger;

        public SecurityDbContext(
            DbContextOptions<SecurityDbContext> options, 
            ILogger<SecurityDbContext> logger) : base(options)
        {
            _logger = logger;
        }

        public DbSet<CacheControl> CacheControls { get; set; }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _logger.LogInformation("SaveChanges() called");

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("SaveChangesAsync() called");

            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // must add this line when scaffold the Identity
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable(name: "User", schema: DbSchemaName);
            });
            modelBuilder.Entity<CustomRole>(entity =>
            {
                entity.ToTable(name: "Role", schema: DbSchemaName);
                entity.Property("_permissionsInRole")
                    .HasColumnName("PermissionsInRole");
            });
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles", DbSchemaName);
            });
            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims", DbSchemaName);
            });
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins", DbSchemaName);
            });
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims", DbSchemaName);
            });
            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens", DbSchemaName);
            });

            modelBuilder.Entity<CacheControl>().ToTable(schema: DbSchemaName, name: "CacheControl");
        }
    }
}
