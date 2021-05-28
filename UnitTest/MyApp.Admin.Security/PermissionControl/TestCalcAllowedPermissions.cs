using Microsoft.AspNetCore.Identity;
using Moq;
using MyApp.Admin.Security.Domains;
using MyApp.Admin.Security.Public.Enums;
using MyApp.Admin.Security.Public.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyApp.Admin.Security.PermissionControl
{
    public class TestCalcAllowedPermissions
    {
        private static List<CustomRole> TestRolesData = new List<CustomRole>
            {
                new CustomRole ("ROLE1", "Role 1", false, new List<Permissions> { Permissions.AccessAll }),
                new CustomRole ("ROLE2", "Role 2", false, new List<Permissions> { Permissions.RoleView, Permissions.RoleAdd, Permissions.RoleEdit, Permissions.RoleDelete }),
                new CustomRole ("ROLE3", "Role 3", false, new List<Permissions> { Permissions.RoleView }),
            };


        public static TheoryData<List<string>, List<Permissions>> TestData1 =>
            new TheoryData<List<string>, List<Permissions>>
            {
                {
                    new List<string> { "ROLE1" },
                    new List<Permissions> { Permissions.AccessAll }
                },
                {
                    new List<string> { "ROLE2", "ROLE3" },
                    new List<Permissions> { Permissions.RoleView, Permissions.RoleAdd, Permissions.RoleEdit, Permissions.RoleDelete }
                },
                {
                    new List<string> {  },
                    new List<Permissions> {  }
                },
                {
                    new List<string> { "ROLE1", "ROLE2" },
                    new List<Permissions> { Permissions.AccessAll, Permissions.RoleView, Permissions.RoleAdd, Permissions.RoleEdit, Permissions.RoleDelete }
                }
            };

        [Theory]
        [MemberData(nameof(TestData1))]
        public async Task TestCalcPermissionsForUserAsync(List<string> testInput, List<Permissions> expected)
        {
            var mockUserStore = new Mock<IUserStore<UserProfile>>();
            mockUserStore.Setup(x => x.FindByIdAsync(It.IsAny<string>(), CancellationToken.None).Result).Returns(
                    new UserProfile
                    {
                        Id = "xxx",
                        UserName = "mock1",
                        Email = "mock1@email.com"
                    }
                );

            var mockUserManager = new Mock<UserManager<UserProfile>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);
            
            mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<UserProfile>()).Result).Returns(
                    testInput
                );

            var mockRoleStore = new Mock<IRoleStore<CustomRole>>();
            
            var mockRoleManager = new Mock<RoleManager<CustomRole>>(
                mockRoleStore.Object, null, null, null, null);
            mockRoleManager.Setup(x => x.Roles).Returns(
                    TestRolesData.AsQueryable()
                );

            var calcPermission = new CalcAllowedPermissions(mockUserManager.Object, mockRoleManager.Object);

            var results = (await calcPermission.CalcPermissionsForUserAsync("anyuser")).UnpackPermissionsFromString();
            
            Assert.Equal(expected, results);

        }
    }
}
