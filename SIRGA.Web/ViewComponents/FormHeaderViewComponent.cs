using Microsoft.AspNetCore.Mvc;

namespace SIRGA.Web.ViewComponents
{
    public class FormHeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(FormHeaderViewModel model)
        {
            return View(model);
        }

    }
    public class FormHeaderViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconPath { get; set; }
        public string ColorScheme { get; set; } // "blue", "purple", "green", etc.

        public FormHeaderViewModel(string title, string description, string iconPath, string colorScheme = "blue")
        {
            Title = title;
            Description = description;
            IconPath = iconPath;
            ColorScheme = colorScheme;
        }

        public string GetGradientClasses()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "from-blue-500 to-blue-600",
                "purple" => "from-purple-500 to-purple-600",
                "green" => "from-green-500 to-green-600",
                "red" => "from-red-500 to-red-600",
                "yellow" => "from-yellow-500 to-yellow-600",
                "indigo" => "from-indigo-500 to-indigo-600",
                _ => "from-gray-500 to-gray-600"
            };
        }
    }
}