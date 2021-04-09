using MyApp.Admin.Security.Public.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.Public.Dtos
{
    public class AllPermissionOptionsDto
    {
        public IEnumerable<string> GroupNameList { get; set; }
        
        public IDictionary<string, List<PermissionOptionItemDto>> OptionsByGroupName { get; set; }
        
    }

}
