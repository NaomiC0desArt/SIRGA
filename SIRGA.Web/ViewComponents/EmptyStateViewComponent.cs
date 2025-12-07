using Microsoft.AspNetCore.Mvc;

namespace SIRGA.Web.ViewComponents
{
    public class EmptyStateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(EmptyStateViewModel model)
        {
            return View(model);
        }
    }

    public class EmptyStateViewModel
    {
        public string IconPath { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public List<ActionButton> Actions { get; set; } = new List<ActionButton>();
        public bool IsFilteredResult { get; set; } = false; // Si es resultado de filtros
    }
}
