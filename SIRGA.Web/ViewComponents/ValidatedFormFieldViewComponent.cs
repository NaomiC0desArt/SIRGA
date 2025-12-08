using Microsoft.AspNetCore.Mvc;

namespace SIRGA.Web.ViewComponents
{
    public class ValidatedFormFieldViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ValidatedFormFieldViewModel model)
        {
            return View(model);
        }
    }

    public class ValidatedFormFieldViewModel
    {
        public string PropertyName { get; set; }
        public string Label { get; set; }
        public string InputType { get; set; } = "text";
        public string IconPath { get; set; }
        public string ColorScheme { get; set; } = "blue";
        public string Placeholder { get; set; }
        public string HelpText { get; set; }
        public bool IsRequired { get; set; }
        public int TextAreaRows { get; set; } = 3;
        public List<SelectOption> Options { get; set; } = new();

        public ValidatedFormFieldViewModel() { }

        public ValidatedFormFieldViewModel(
            string propertyName,
            string label,
            string iconPath,
            string inputType = "text",
            string colorScheme = "blue")
        {
            PropertyName = propertyName;
            Label = label;
            IconPath = iconPath;
            InputType = inputType;
            ColorScheme = colorScheme;
        }

        public string GetIconColorClass()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "text-blue-500",
                "purple" => "text-purple-500",
                "green" => "text-green-500",
                "red" => "text-red-500",
                "yellow" => "text-yellow-500",
                "indigo" => "text-indigo-500",
                _ => "text-gray-500"
            };
        }

        public string GetFocusRingClass()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "focus:ring-blue-500 focus:border-blue-500",
                "purple" => "focus:ring-purple-500 focus:border-purple-500",
                "green" => "focus:ring-green-500 focus:border-green-500",
                "red" => "focus:ring-red-500 focus:border-red-500",
                "yellow" => "focus:ring-yellow-500 focus:border-yellow-500",
                "indigo" => "focus:ring-indigo-500 focus:border-indigo-500",
                _ => "focus:ring-gray-500 focus:border-gray-500"
            };
        }
    }

    public class SelectOption
    {
        public string Value { get; set; }
        public string Text { get; set; }

        public SelectOption(string value, string text)
        {
            Value = value;
            Text = text;
        }
    }
}
