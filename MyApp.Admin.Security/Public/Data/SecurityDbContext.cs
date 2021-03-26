using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyApp.Admin.Security.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SecurityDbContext : IdentityDbContext<UserProfile>
    {
        public static readonly string DbSchemaName = "Security";
        
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // must add this line when scaffold the Identity
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable(name: "User", schema: DbSchemaName);
            });
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role", schema: DbSchemaName);
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

        }
    }
}
