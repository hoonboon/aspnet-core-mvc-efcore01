using MyApp.Admin.Security.Public.Enums;
using MyApp.Common.Public.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MyApp.Admin.Security.Public.Dtos
{
    public class RoleDetailDto
    {
        public string RoleId { get; set; }

        [Display(Name = "Role Name")]
        public string Name { get; set; }

        [Display(Name = "Role Decription")]
        public string RoleDescription { get; set; }

        public bool IsBuiltInRole { get; set; } = false;

        [Display(Name = "Permissions")]
        public IEnumerable<Permissions> PermissionsInput { get; set; }

        public AllPermissionOptionsDto PermissionOptions { get; set; }

    }
}
