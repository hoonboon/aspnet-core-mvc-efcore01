using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspnetCoreWebMvcApp03.Migrations
{
    public partial class Inheritance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignment_Instructor_InstructorId",
                table: "CourseAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Instructor_InstructorId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_Student_StudentId",
                table: "Enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_OfficeAssignment_Instructor_InstructorId",
                table: "OfficeAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Student",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Student");

            migrationBuilder.RenameTable(
                name: "Student",
                newName: "Person");

            migrationBuilder.AddColumn<DateTime>(
                name: "HireDate",
                table: "Person",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Person",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Person",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Person",
                table: "Person",
                column: "Id");

            /** 
             * Custom migration required to copy existing data in table "Instructor" into the new table "Person"
             * before dropping table "Instructor"
             */
            migrationBuilder.AddColumn<int>(name: "OldId", table: "Person", nullable: true);

            // Copy existing data into new table.
            migrationBuilder.Sql(
                "INSERT INTO dbo.Person (LastName, FirstName, HireDate, EnrollmentDate, Discriminator, OldId)" 
                + " SELECT LastName, FirstName, HireDate, null AS EnrollmentDate, 'Instructor' AS Discriminator, InstructorId AS OldId"
                + " FROM dbo.Instructor");
            // Fix up existing relationships to match new PK's.
            migrationBuilder.Sql(
                "UPDATE dbo.CourseAssignment" 
                + " SET InstructorId = ("
                + "     SELECT Id FROM dbo.Person WHERE OldId = CourseAssignment.InstructorId AND Discriminator = 'Instructor'"
                + " )");
            migrationBuilder.Sql(
                "UPDATE dbo.Department"
                + " SET InstructorId = ("
                + "     SELECT Id FROM dbo.Person WHERE OldId = Department.InstructorId AND Discriminator = 'Instructor'"
                + " )");
            migrationBuilder.Sql(
                "UPDATE dbo.OfficeAssignment"
                + " SET InstructorId = ("
                + "     SELECT Id FROM dbo.Person WHERE OldId = OfficeAssignment.InstructorId AND Discriminator = 'Instructor'"
                + " )");

            // Remove temporary key
            migrationBuilder.DropColumn(name: "OldId", table: "Person");

            migrationBuilder.DropTable(
                name: "Instructor");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignment_Person_InstructorId",
                table: "CourseAssignment",
                column: "InstructorId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Person_InstructorId",
                table: "Department",
                column: "InstructorId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_Person_StudentId",
                table: "Enrollment",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OfficeAssignment_Person_InstructorId",
                table: "OfficeAssignment",
                column: "InstructorId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignment_Person_InstructorId",
                table: "CourseAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Person_InstructorId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_Person_StudentId",
                table: "Enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_OfficeAssignment_Person_InstructorId",
                table: "OfficeAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Person",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "HireDate",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Person");

            migrationBuilder.RenameTable(
                name: "Person",
                newName: "Student");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Student",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Student",
                table: "Student",
                column: "StudentId");

            migrationBuilder.CreateTable(
                name: "Instructor",
                columns: table => new
                {
                    InstructorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructor", x => x.InstructorId);
                });

            /** 
             * TODO: Custom migration required to move back existing data in table "Person" into the new table "Instructor"
             */

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignment_Instructor_InstructorId",
                table: "CourseAssignment",
                column: "InstructorId",
                principalTable: "Instructor",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Instructor_InstructorId",
                table: "Department",
                column: "InstructorId",
                principalTable: "Instructor",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_Student_StudentId",
                table: "Enrollment",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OfficeAssignment_Instructor_InstructorId",
                table: "OfficeAssignment",
                column: "InstructorId",
                principalTable: "Instructor",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
