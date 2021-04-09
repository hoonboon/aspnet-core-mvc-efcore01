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

        // Administration - Role Management
        [Display(GroupName = "Role Management", Name = "View", Description = "Can view Roles")]
        RoleView = 10,
        [Display(GroupName = "Role Management", Name = "Add", Description = "Can add new Role")]
        RoleAdd = 11,
        [Display(GroupName = "Role Management", Name = "Edit", Description = "Can edt Role")]
        RoleEdit = 12,
        [Display(GroupName = "Role Management", Name = "Delete", Description = "Can delete Role")]
        RoleDelete = 13,

        // Super Administrator
        [Display(GroupName = "Super Admin", Name = "Access All", Description = "All permissions allowed")]
        AccessAll = Int16.MaxValue,
    }
}
