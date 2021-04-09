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
            UserManager<UserProfile> userManager, RoleManager<CustomRole> roleManager,
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
            await roleManager.CreateAsync(
                new CustomRole(
                    Roles.SuperAdmin.ToString(), "Super Admin", true, 
                    new [] { Permissions.AccessAll }));
            await roleManager.CreateAsync(
                new CustomRole(
                    Roles.Admin.ToString(), "Admin", true,
                    new [] { Permissions.None }));
            await roleManager.CreateAsync(
                new CustomRole(
                    Roles.Manager.ToString(), "Manager", true,
                    new [] { Permissions.None }));
            await roleManager.CreateAsync(
                new CustomRole(
                    Roles.Staff.ToString(), "Staff", true,
                    new [] { Permissions.None }));

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
