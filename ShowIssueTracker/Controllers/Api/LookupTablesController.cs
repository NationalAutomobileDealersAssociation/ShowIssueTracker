using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShowIssueTracker.Authorization;
using ShowIssueTracker.Data;
using ShowIssueTracker.Models;
using ShowIssueTracker.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ShowIssueTracker.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class LookupTablesController : Controller
    {
        private readonly IGlobalExceptionLoggingMiddleWare _logging;
      
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImpexiumLogin _impexiumLogin;

        public LookupTablesController( IGlobalExceptionLoggingMiddleWare logging,
            UserManager<ApplicationUser> userManager, IImpexiumLogin impexiumLogin)
        {
            _logging = logging; 
            _userManager = userManager;
            _impexiumLogin = impexiumLogin;
        }


        [HttpGet]
        public async Task<IActionResult> PopulateUsers(string keyword)
        {
            var user = _userManager.GetUserName(User);
            if (user == null)
            {
                return Json(new { success = false, value = "Please Log in to Continue." });
            }

            List<UserAccess> dealerships = new List<UserAccess>();
            try
            {
                dealerships = await _impexiumLogin.GetListOfUsers(keyword);

                var data = dealerships.Select(item => new SelectSearch()
                {
                    Id = item.Id,
                    Text = item.Name
                }).Where(d => d.Id != null);

                var result = data.GroupBy(d => d.Text).Select(d => d.First()).OrderBy(d => d.Text);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logging.Invoke(ex, "Failed to get dealerships.");
                return Json(new { success = false, value = "Failed to get dealerships." });
            }


        }
    }
}