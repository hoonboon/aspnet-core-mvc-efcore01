using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyApp.Admin.Security.Public.Dtos
{
    public class RoleListDto
    {
        public string RoleId { get; set; }

        [Display(Name = "Role Name")]
        public string Name { get; set; }

        [Display(Name = "Role Decription")]
        public string RoleDescription { get; set; }

        public bool IsBuiltInRole { get; set; }
    }
}
