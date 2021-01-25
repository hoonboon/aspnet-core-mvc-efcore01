using AspnetCoreWebMvcApp03.Areas.Identity.Data;
using AspnetCoreWebMvcApp03.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreWebMvcApp03.Data
{
    // must extend IdentityDbContext first before being re-used to Scaffold Identity
    public class SchoolContext : IdentityDbContext<AppUser, AppRole, string,
        AppUserClaim, AppUserRole, AppUserLogin,
        AppRoleClaim, AppUserToken>
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // must add this line when scaffold the Identity
            base.OnModelCreating(modelBuilder);

            // Identity Tables
            modelBuilder.Entity<AppUser>(b =>
            {
                b.ToTable("AppUser");

                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne(e => e.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne(e => e.User)
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<AppUserClaim>(b =>
            {
                b.ToTable("AppUserClaim");
            });

            modelBuilder.Entity<AppUserLogin>(b =>
            {
                b.ToTable("AppUserLogin");
            });

            modelBuilder.Entity<AppUserToken>(b =>
            {
                b.ToTable("AppUserToken");
            });

            modelBuilder.Entity<AppRole>(b =>
            {
                b.ToTable("AppRole");

                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                // Each Role can have many associated RoleClaims
                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
            });

            modelBuilder.Entity<AppRoleClaim>(b =>
            {
                b.ToTable("AppRoleClaim");
            });

            modelBuilder.Entity<AppUserRole>(b =>
            {
                b.ToTable("AppUserRole");
            });
            
            // App Tables
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Instructor>().ToTable("Instructor");
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");
            
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment")
                .HasKey(c => new { c.CourseId, c.InstructorId });

        }
    }
}
