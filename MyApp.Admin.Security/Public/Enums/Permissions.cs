using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.Public.Enums
{
    public enum Permissions : short
    {
        // Default when no permissions assigned, should not be displayed as a permission option
        [Obsolete]
        None = 0, //error condition

        // Admin: Roles
        [Display(GroupName = "Admin: Roles", Name = "View", Description = "Can view Roles")]
        RoleView = 100,
        [Display(GroupName = "Admin: Roles", Name = "Add", Description = "Can add new Role")]
        RoleAdd = 101,
        [Display(GroupName = "Admin: Roles", Name = "Edit", Description = "Can edit Role")]
        RoleEdit = 102,
        [Display(GroupName = "Admin: Roles", Name = "Delete", Description = "Can delete Role")]
        RoleDelete = 103,

        // TODO: Administration - User Management


        // Admin: User Roles
        [Display(GroupName = "Admin: User Roles", Name = "View", Description = "Can view User Roles")]
        UserRoleView = 200,
        [Display(GroupName = "Admin: User Roles", Name = "Manage", Description = "Can manage User Role")]
        UserRoleManage = 201,

        // School - Students
        [Display(GroupName = "School: Students", Name = "View", Description = "Can view Students")]
        StudentView = 300,
        [Display(GroupName = "School: Students", Name = "Add", Description = "Can add new Student")]
        StudentAdd = 301,
        [Display(GroupName = "School: Students", Name = "Edit", Description = "Can edit Student")]
        StudentEdit = 302,
        [Display(GroupName = "School: Students", Name = "Delete", Description = "Can delete Student")]
        StudentDelete = 303,

        // School: Courses
        [Display(GroupName = "School: Courses", Name = "View", Description = "Can view Courses")]
        CourseView = 400,
        [Display(GroupName = "School: Courses", Name = "Add", Description = "Can add new Course")]
        CourseAdd = 401,
        [Display(GroupName = "School: Courses", Name = "Edit", Description = "Can edit Course")]
        CourseEdit = 402,
        [Display(GroupName = "School: Courses", Name = "Delete", Description = "Can delete Course")]
        CourseDelete = 403,

        // School: Instructors
        [Display(GroupName = "School: Instructors", Name = "View", Description = "Can view Instructors")]
        InstructorView = 500,
        [Display(GroupName = "School: Instructors", Name = "Add", Description = "Can add new Instructor")]
        InstructorAdd = 501,
        [Display(GroupName = "School: Instructors", Name = "Edit", Description = "Can edit Instructor")]
        InstructorEdit = 502,
        [Display(GroupName = "School: Instructors", Name = "Delete", Description = "Can delete Instructor")]
        InstructorDelete = 503,

        // School: Departments
        [Display(GroupName = "School: Departments", Name = "View", Description = "Can view Departments")]
        DepartmentView = 600,
        [Display(GroupName = "School: Departments", Name = "Add", Description = "Can add new Department")]
        DepartmentAdd = 601,
        [Display(GroupName = "School: Departments", Name = "Edit", Description = "Can edit Department")]
        DepartmentEdit = 602,
        [Display(GroupName = "School: Departments", Name = "Delete", Description = "Can delete Department")]
        DepartmentDelete = 603,


        // Super Administrator
        [Display(GroupName = "Super Admin", Name = "Access All", Description = "All permissions allowed")]
        AccessAll = Int16.MaxValue,
    }
}
