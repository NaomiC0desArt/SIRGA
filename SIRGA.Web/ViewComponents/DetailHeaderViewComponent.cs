using Microsoft.AspNetCore.Mvc;

namespace SIRGA.Web.ViewComponents
{
    public class DetailHeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DetailHeaderViewModel model)
        {
            return View(model);
        }
    }
    public class DetailHeaderViewModel
    {
        public string Title { get; set; }
        public string IconPath { get; set; }
        public string ColorScheme { get; set; } = "blue";
        public List<StatusBadge> Badges { get; set; } = new();
        public List<ActionButton> ActionButtons { get; set; } = new();

        public DetailHeaderViewModel(string title, string iconPath, string colorScheme = "blue")
        {
            Title = title;
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
                _ => "from-gray-500 to-gray-600"
            };
        }
    }

    public class StatusBadge
    {
        public string Text { get; set; }
        public string Color { get; set; } // "green", "red", "blue", "purple", etc.

        public StatusBadge(string text, string color)
        {
            Text = text;
            Color = color;
        }

        public string GetBadgeClasses()
        {
            return Color.ToLower() switch
            {
                "green" => "bg-green-100 text-green-800",
                "red" => "bg-red-100 text-red-800",
                "blue" => "bg-blue-100 text-blue-800",
                "purple" => "bg-purple-100 text-purple-800",
                "yellow" => "bg-yellow-100 text-yellow-800",
                _ => "bg-gray-100 text-gray-800"
            };
        }
    }
}
