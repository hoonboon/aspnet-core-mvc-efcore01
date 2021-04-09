using Microsoft.AspNetCore.Identity;
using MyApp.Admin.Security.Public.Enums;
using MyApp.Admin.Security.Public.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MyApp.Admin.Security.Domains
{
    public class CustomRole : IdentityRole
    {
        [Required(AllowEmptyStrings = false)]
        public string RoleDescription { get; set; }
        
        public bool IsBuiltInRole { get; set; } = false;

        [Required(AllowEmptyStrings = false)] // A role must have at least one permission in it
        private string _permissionsInRole;

        public IEnumerable<Permissions> PermissionsInRole => _permissionsInRole.UnpackPermissionsFromString();

        private CustomRole() : base()
        {
        }

        public CustomRole(string roleName) : base(roleName)
        {
            this.RoleDescription = roleName;
        }

        public CustomRole(
            string roleName, string roleDescription, bool isBuilInRole,
            IEnumerable<Permissions> permissions) : base(roleName)
        {
            this.RoleDescription = roleDescription;
            this.IsBuiltInRole = isBuilInRole;

            UpdatePermissionsInRole(permissions);
        }

        public void UpdatePermissionsInRole(IEnumerable<Permissions> updatedPermissions)
        {
            if (updatedPermissions == null || !updatedPermissions.Any())
                throw new InvalidOperationException("There should be at least one permission associated with a role.");

            this._permissionsInRole = updatedPermissions.PackPermissionsIntoString();
        }
    }
}
