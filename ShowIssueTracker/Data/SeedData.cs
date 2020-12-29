using ShowIssueTracker.Models;
using ShowIssueTracker;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowIssueTracker.Data
{
    public static class SeedData
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            //var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //var appSettings = serviceProvider.GetRequiredService<IOptions<GlobalConstants>>();
            //var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            //await roleManager.EnsureRolesCreated();

            //var superUserEmail = appSettings.Value.SuperUserEmail;
            //var superUserPassword = appSettings.Value.SuperUserPassword;
            //var superUser = await userManager.FindByEmailAsync(superUserEmail);

            //if (superUser != null) return;
            //superUser = new ApplicationUser()
            //{
            //    UserName = superUserEmail,
            //    Email = superUserEmail
            //};

            //var result = await userManager.CreateAsync(superUser, superUserPassword);

            //if (result.Succeeded)
            //{
            //    // Add user to Admin Role
            //    await userManager.AddToRoleAsync(superUser, RoleConstants.admin);
            //}
        }
        private static async Task EnsureRoleCreated(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public static async Task EnsureRolesCreated(this RoleManager<IdentityRole> roleManager)
        {
            await EnsureRoleCreated(roleManager, RoleConstants.admin);
            await EnsureRoleCreated(roleManager, RoleConstants.consultant);
            await EnsureRoleCreated(roleManager, RoleConstants.guest);
            await EnsureRoleCreated(roleManager, RoleConstants.approver);
        }
        public class RoleConstants
        {
            public static string admin = "Admin";
            public static string consultant = "Consultant";
            public static string guest = "Guest";
            public static string approver  = "Approver";
        }
    }
}
