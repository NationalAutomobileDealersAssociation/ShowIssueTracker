using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShowIssueTracker.ViewComponents
{
    public class TopNavViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {

            return View();
        }
    }
}
