using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.Components;

namespace SIRGA.Web.ViewComponents
{
    public class PageHeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(PageHeaderViewModel model)
        {
            return View(model);
        }
    }

    public class PageHeaderViewModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string IconPath { get; set; }
        public string ColorScheme { get; set; } = "blue"; // blue, purple, green, etc.
        public List<ActionButton> ActionButtons { get; set; } = new List<ActionButton>();

        public string GetGradientClasses()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "from-blue-500 to-blue-600",
                "purple" => "from-purple-500 to-purple-600",
                "green" => "from-green-500 to-green-600",
                "indigo" => "from-indigo-500 to-indigo-600",
                "red" => "from-red-500 to-red-600",
                _ => "from-gray-500 to-gray-600"
            };
        }
    }

    public class ActionButton
    {
        public string Text { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string IconPath { get; set; }
        public string ColorClass { get; set; } = "bg-gradient-to-r from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700 text-white"; // Clases CSS completas
        public Dictionary<string, string> RouteValues { get; set; } = new Dictionary<string, string>();
    }
}
