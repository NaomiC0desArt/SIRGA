using Microsoft.AspNetCore.Mvc;

namespace SIRGA.Web.ViewComponents
{
    public class DashboardHeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DashboardHeaderViewModel model)
        {
            return View(model);
        }
    }

    public class DashboardHeaderViewModel
    {
        public string Title { get; set; }
        public string UserName { get; set; }
        public string ColorScheme { get; set; } // "blue", "purple", etc.
        public string IconPath { get; set; }
        public bool ShowDate { get; set; } = true;

        public string GetGradientClasses()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "from-blue-500 to-blue-600",
                "purple" => "from-purple-500 to-purple-600",
                "green" => "from-green-500 to-green-600",
                _ => "from-blue-500 to-blue-600"
            };
        }

        public string GetDateBgClass()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "bg-blue-50 border-blue-200 text-blue-700",
                "purple" => "bg-purple-50 border-purple-200 text-purple-700",
                "green" => "bg-green-50 border-green-200 text-green-700",
                _ => "bg-blue-50 border-blue-200 text-blue-700"
            };
        }
    }
}
