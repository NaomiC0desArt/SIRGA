using Microsoft.AspNetCore.Mvc;

namespace SIRGA.Web.ViewComponents
{
    public class FormFieldViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(FormFieldViewModel model)
        {
            return View(model);
        }
    }

    public class FormFieldViewModel
    {
        public string FieldName { get; set; } // Nombre del campo para asp-for
        public string Label { get; set; }
        public string IconPath { get; set; }
        public string IconColor { get; set; } = "blue"; // blue, purple, green, etc.
        public string InputType { get; set; } = "text"; // text, email, number, textarea, select
        public string Placeholder { get; set; }
        public bool IsRequired { get; set; } = false;
        public string HelpText { get; set; }
        public int? TextAreaRows { get; set; } = 3;
        public object SelectOptions { get; set; } // Para selects

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

        public string GetFocusRingClass()
        {
            return IconColor.ToLower() switch
            {
                "blue" => "focus:ring-blue-500 focus:border-blue-500",
                "purple" => "focus:ring-purple-500 focus:border-purple-500",
                "green" => "focus:ring-green-500 focus:border-green-500",
                "red" => "focus:ring-red-500 focus:border-red-500",
                _ => "focus:ring-blue-500 focus:border-blue-500"
            };
        }
    }
}
