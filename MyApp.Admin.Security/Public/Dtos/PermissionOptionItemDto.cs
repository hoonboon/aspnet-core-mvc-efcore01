using MyApp.Admin.Security.Public.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.Public.Dtos
{
    public class PermissionOptionItemDto
    {
        public string Label { get; set; }
        public Permissions Value { get; set; }
        public bool IsSelected { get; set; } = false;
    }


}
