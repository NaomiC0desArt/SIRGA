using Microsoft.AspNetCore.Mvc;

namespace SIRGA.Web.ViewComponents
{
    public class FormActionsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(FormActionsViewModel model)
        {
            return View(model);
        }
    }
    public class FormActionsViewModel
    {
        public string CancelAction { get; set; }
        public string CancelController { get; set; }
        public string SubmitText { get; set; }
        public string SubmitIconPath { get; set; }
        public string ColorScheme { get; set; }
        public bool ShowBorder { get; set; } = true;

        public FormActionsViewModel(
            string cancelAction,
            string submitText,
            string colorScheme = "blue",
            string cancelController = null)
        {
            CancelAction = cancelAction;
            CancelController = cancelController;
            SubmitText = submitText;
            ColorScheme = colorScheme;
            SubmitIconPath = "M5 13l4 4L19 7"; // Default checkmark icon
        }

        public string GetGradientClasses()
        {
            return ColorScheme.ToLower() switch
            {
                "blue" => "from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700",
                "purple" => "from-purple-500 to-purple-600 hover:from-purple-600 hover:to-purple-700",
                "green" => "from-green-500 to-green-600 hover:from-green-600 hover:to-green-700",
                "red" => "from-red-500 to-red-600 hover:from-red-600 hover:to-red-700",
                _ => "from-gray-500 to-gray-600 hover:from-gray-600 hover:to-gray-700"
            };
        }
    }
}
