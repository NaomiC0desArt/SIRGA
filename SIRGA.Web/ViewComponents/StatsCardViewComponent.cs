using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.Components;

namespace SIRGA.Web.ViewComponents
{
    public class StatsCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(StatsCardViewModel model)
        {
            return View(model);
        }
    }
    public class StatsCardViewModel
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string ColorScheme { get; set; } // "blue", "green", "orange", "purple"
        public string IconPath { get; set; } // SVG path data

        // Método helper para obtener las clases de Tailwind según el color
        public string GetGradientClasses()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "from-blue-500 to-blue-600",
                "green" => "from-green-500 to-green-600",
                "orange" => "from-orange-500 to-orange-600",
                "purple" => "from-purple-500 to-purple-600",
                _ => "from-gray-500 to-gray-600"
            };
        }

        public string GetTextColorClass()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "text-blue-100",
                "green" => "text-green-100",
                "orange" => "text-orange-100",
                "purple" => "text-purple-100",
                _ => "text-gray-100"
            };
        }
    }
    }
