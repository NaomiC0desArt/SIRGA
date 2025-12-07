using Microsoft.AspNetCore.Mvc;

namespace SIRGA.Web.ViewComponents
{
    public class InfoCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(InfoCardViewModel model)
        {
            return View(model);
        }
    }

    public class InfoCardViewModel
    {
        public string Title { get; set; }
        public string IconPath { get; set; }
        public string IconColor { get; set; } = "blue";
        public List<InfoField> Fields { get; set; } = new List<InfoField>();
        public int? GridColumns { get; set; } = 1; // Para especificar cuántos campos por fila

        public string GetIconColorClass()
        {
            return IconColor.ToLower() switch
            {
                "blue" => "text-blue-500",
                "purple" => "text-purple-500",
                "green" => "text-green-500",
                "red" => "text-red-500",
                "orange" => "text-orange-500",
                _ => "text-gray-500"
            };
        }
    }

    public class InfoField
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public bool IsEmpty { get; set; } = false;
        public string EmptyMessage { get; set; } = "No completado";
        public bool IsBadge { get; set; } = false; // Para mostrar como badge
        public string BadgeColor { get; set; } = "blue"; // blue, green, purple, yellow, red
    }
}
