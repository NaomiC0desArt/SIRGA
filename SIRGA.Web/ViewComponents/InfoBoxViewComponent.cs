using Microsoft.AspNetCore.Mvc;

namespace SIRGA.Web.ViewComponents
{
    public class InfoBoxViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(InfoBoxViewModel model)
        {
            return View(model);
        }
    }

    public class InfoBoxViewModel
    {
        public string Title { get; set; }
        public List<string> Items { get; set; } = new();
        public string ColorScheme { get; set; } = "blue";
        public string IconPath { get; set; } = "M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z";

        public InfoBoxViewModel() { }

        public InfoBoxViewModel(string title, List<string> items, string colorScheme = "blue")
        {
            Title = title;
            Items = items ?? new List<string>();
            ColorScheme = colorScheme;
        }

        public string GetBackgroundClass()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "bg-blue-50 border-blue-200",
                "purple" => "bg-purple-50 border-purple-200",
                "green" => "bg-green-50 border-green-200",
                "yellow" => "bg-yellow-50 border-yellow-200",
                "red" => "bg-red-50 border-red-200",
                _ => "bg-gray-50 border-gray-200"
            };
        }

        public string GetTextColorClass()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "text-blue-700",
                "purple" => "text-purple-700",
                "green" => "text-green-700",
                "yellow" => "text-yellow-700",
                "red" => "text-red-700",
                _ => "text-gray-700"
            };
        }

        public string GetIconColorClass()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "text-blue-500",
                "purple" => "text-purple-500",
                "green" => "text-green-500",
                "yellow" => "text-yellow-500",
                "red" => "text-red-500",
                _ => "text-gray-500"
            };
        }
    }
}
