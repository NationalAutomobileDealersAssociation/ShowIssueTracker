using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowIssueTracker.Authorization
{
    public class ClaimsRequirement : IAuthorizationRequirement
    {
        public ClaimsRequirement(string permission)
        {
            Permission = permission;
        }
        public string Permission { get; set; }
    }

}
