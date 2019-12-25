using Microsoft.AspNetCore.Mvc;

namespace CatsReviewApp.Components
{
    public class StatusLabelViewComponent : ViewComponent
    {
        public IViewComponentResult InvokeAsync(bool? isApproved)
        {
            return View(isApproved);
        }
    }
}
