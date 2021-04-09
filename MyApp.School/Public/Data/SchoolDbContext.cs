using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApp.School.Domains;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace MyApp.School.Public.Data
{
    /**
     * Notes on EF Migrations:
     * 
     * Execute the following command in a Developer Command Prompt from the startup project directory: MyApp.WebMvc03
     * 
     * Sample Command: 
     * - Create Migration: ...\MyApp.WebMvc03>dotnet ef migrations add InitialCreate --context SchoolDbContext --project "../MyApp.School/MyApp.School.csproj"
     * - Update Database: ...\MyApp.WebMvc03>dotnet ef database update --context SchoolDbContext --project "../MyApp.School/MyApp.School.csproj"
     * 
     */

    // must extend IdentityDbContext first before being re-used to Scaffold Identity
    public class SchoolDbContext : DbContext
    {
        public static readonly string DbSchemaName = "School";

        private readonly ILogger<SchoolDbContext> _logger;

        public SchoolDbContext(
            DbContextOptions<SchoolDbContext> options,
            ILogger<SchoolDbContext> logger) : base(options)
        {
            _logger = logger;
        }

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

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable(schema: DbSchemaName, name: "Course");
            modelBuilder.Entity<Enrollment>().ToTable(schema: DbSchemaName, name: "Enrollment");
            modelBuilder.Entity<Student>().ToTable(schema: DbSchemaName, name: "Student");
            modelBuilder.Entity<Department>().ToTable(schema: DbSchemaName, name: "Department");
            modelBuilder.Entity<Instructor>().ToTable(schema: DbSchemaName, name: "Instructor");
            modelBuilder.Entity<OfficeAssignment>().ToTable(schema: DbSchemaName, name: "OfficeAssignment");
            
            modelBuilder.Entity<CourseAssignment>().ToTable(schema: DbSchemaName, name: "CourseAssignment")
                .HasKey(c => new { c.CourseId, c.InstructorId });

        }
    }
}
