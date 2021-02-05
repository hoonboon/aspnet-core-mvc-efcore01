using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreWebMvcApp03.Models.AdminViewModels
{
    public class UserRoleAssignedData
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsAssigned { get; set; }
    }
}
