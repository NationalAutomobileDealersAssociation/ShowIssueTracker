using ShowIssueTracker.Data;
using ShowIssueTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ShowIssueTracker.Authorization
{
    public class ClaimsHandler : AuthorizationHandler<ClaimsRequirement>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ClaimsHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ClaimsRequirement requirement)
        {
            // ADMIN Role 
            // - Allow All Access
            if (context.User.IsInRole(SeedData.RoleConstants.admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // USER Role 
            // - Allow View/Edit Access
            if (context.User.IsInRole(SeedData.RoleConstants.consultant))
            {
                if (requirement.Permission == "View" || requirement.Permission == "Edit")
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }

            // GUEST Role
            // - Allow View Only Access
            if (context.User.IsInRole(SeedData.RoleConstants.guest))
            {
                if (requirement.Permission == "View")
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }

            return Task.CompletedTask;

        }

    }
}