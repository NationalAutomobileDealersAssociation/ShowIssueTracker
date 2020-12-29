using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShowIssueTracker.ViewComponents
{
    public class LeftNavViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
