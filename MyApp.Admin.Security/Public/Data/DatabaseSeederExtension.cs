using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Admin.Security.Domains;
using MyApp.Admin.Security.Public.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.Public.Data
{
    public static class DatabaseSeederExtension
    {
        public static async Task SeedDatabaseWithSecurityDataAsync(this SecurityDbContext context,
            UserManager<UserProfile> userManager, RoleManager<IdentityRole> roleManager,
            string defaultUserPwd)
        {
            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception)
            {
                throw;
            }

            // Look for any students.
            if (context.Roles.Any())
            {
                return;   // DB has been seeded
            }

            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Manager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Staff.ToString()));

            //Seed Default User
            var defaultUser = new UserProfile
            {
                UserName = "superadmin",
                Email = "superadmin@e.mail",
                FirstName = "Super",
                LastName = "Admin",
                DOB = DateTime.Now,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultUserPwd);
                    await userManager.AddToRoleAsync(defaultUser, Roles.Staff.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Manager.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
                }

            }

            await context.SaveChangesAsync();
        }
    }
}
