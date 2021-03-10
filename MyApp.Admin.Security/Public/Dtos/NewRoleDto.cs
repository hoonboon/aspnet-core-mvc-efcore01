using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Admin.Security.Public.Dtos
{
    public class NewRoleDto
    {
        public string RoleName { get; private set; }

        public NewRoleDto(string roleName)
        {
            RoleName = roleName;
        }
    }
}
