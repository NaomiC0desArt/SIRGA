using Microsoft.AspNetCore.Mvc;

namespace SIRGA.Web.ViewComponents
{
    public class NavigationCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(NavigationCardViewModel model)
        {
            return View(model);
        }
    }
    public class NavigationCardViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ColorScheme { get; set; } // "blue", "purple", "green", "indigo", "teal", "pink"
        public string IconPath { get; set; }

        public string GetBackgroundClasses()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "from-blue-50 to-blue-100 hover:from-blue-100 hover:to-blue-200 border-blue-200",
                "purple" => "from-purple-50 to-purple-100 hover:from-purple-100 hover:to-purple-200 border-purple-200",
                "green" => "from-green-50 to-green-100 hover:from-green-100 hover:to-green-200 border-green-200",
                "indigo" => "from-indigo-50 to-indigo-100 hover:from-indigo-100 hover:to-indigo-200 border-indigo-200",
                "teal" => "from-teal-50 to-teal-100 hover:from-teal-100 hover:to-teal-200 border-teal-200",
                "pink" => "from-pink-50 to-pink-100 hover:from-pink-100 hover:to-pink-200 border-pink-200",
                "orange" => "from-orange-50 to-orange-100 hover:from-orange-100 hover:to-orange-200 border-orange-200",
                _ => "from-gray-50 to-gray-100 hover:from-gray-100 hover:to-gray-200 border-gray-200"
            };
        }

        public string GetIconBgClass()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "bg-blue-500",
                "purple" => "bg-purple-500",
                "green" => "bg-green-500",
                "indigo" => "bg-indigo-500",
                "teal" => "bg-teal-500",
                "pink" => "bg-pink-500",
                "orange" => "bg-orange-500",
                _ => "bg-gray-500"
            };
        }

        public string GetHoverTextClass()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "group-hover:text-blue-700",
                "purple" => "group-hover:text-purple-700",
                "green" => "group-hover:text-green-700",
                "indigo" => "group-hover:text-indigo-700",
                "teal" => "group-hover:text-teal-700",
                "pink" => "group-hover:text-pink-700",
                "orange" => "group-hover:text-orange-700",
                _ => "group-hover:text-gray-700"
            };
        }
    }
}
