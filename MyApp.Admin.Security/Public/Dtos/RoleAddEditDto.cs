using MyApp.Admin.Security.Public.Enums;
using MyApp.Common.Public.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MyApp.Admin.Security.Public.Dtos
{
    public class RoleAddEditDto
    {
        public string RoleId { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 3)]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Role Decription")]
        public string RoleDescription { get; set; }
        
        [Display(Name = "Permissions")]
        public IEnumerable<Permissions> PermissionsInput { get; set; }

        public AllPermissionOptionsDto PermissionOptions { get; set; }

        public static AllPermissionOptionsDto GeneratePermissionOptions(IEnumerable<Permissions> permissionsSelected)
        {
            var permissionList = Enum.GetValues(typeof(Permissions));

            var groupNameList = new List<string>();
            var optionsByGroupName = new Dictionary<string, List<PermissionOptionItemDto>>();
            foreach (Permissions permission in permissionList)
            {
                var tempEnumInfo = permission.GetEnumItemDisplayValues();
                if (!tempEnumInfo.IsObsolete)
                {
                    string tempGroupName = tempEnumInfo.GroupName;

                    if (!optionsByGroupName.TryGetValue(tempGroupName, out List<PermissionOptionItemDto> tempOptionList))
                    {
                        tempOptionList = new List<PermissionOptionItemDto>();
                        groupNameList.Add(tempGroupName);
                        optionsByGroupName.Add(tempGroupName, tempOptionList);
                    }

                    tempOptionList.Add(new PermissionOptionItemDto
                    {
                        Label = tempEnumInfo.Name,
                        Value = permission,
                        IsSelected = permissionsSelected.Contains(permission)
                    });
                }
            }

            return new AllPermissionOptionsDto
            {
                GroupNameList = groupNameList,
                OptionsByGroupName = optionsByGroupName
            };
        }
    }
}
