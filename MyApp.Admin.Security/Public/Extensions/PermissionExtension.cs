using MyApp.Admin.Security.Public.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Admin.Security.Public.Extensions
{
    public static class PermissionExtension
    {
        public static string PackPermissionsIntoString(this IEnumerable<Permissions> permissions)
        {
            return permissions.Aggregate("", (s, permission) => s + (char)permission);
        }

        public static IEnumerable<Permissions> UnpackPermissionsFromString(this string packedPermissions)
        {
            if (packedPermissions == null)
                throw new ArgumentNullException(nameof(packedPermissions));
            foreach (var character in packedPermissions)
            {
                yield return ((Permissions)character);
            }
        }

        public static Permissions? FindPermissionViaName(this string permissionName)
        {
            return 
                Enum.TryParse(permissionName, out Permissions permission) 
                ? (Permissions?)permission : null;
        }
    }
}
