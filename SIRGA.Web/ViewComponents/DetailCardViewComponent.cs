using Microsoft.AspNetCore.Mvc;

namespace SIRGA.Web.ViewComponents
{
    public class DetailCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DetailCardViewModel model)
        {
            return View(model);
        }
    }
    public class DetailCardViewModel
        {
            public string Title { get; set; }
            public string IconPath { get; set; }
            public string ColorScheme { get; set; } = "blue";
            public int ColumnSpan { get; set; } = 1; // 1 o 2
            public List<DetailField> Fields { get; set; } = new();

            public DetailCardViewModel() { }

            public DetailCardViewModel(string title, string iconPath, string colorScheme = "blue")
            {
                Title = title;
                IconPath = iconPath;
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
                    _ => "text-gray-500"
                };
            }

            public string GetColSpanClass()
            {
                return ColumnSpan == 2 ? "md:col-span-2" : "";
            }
        }

        public class DetailField
        {
            public string Label { get; set; }
            public string Value { get; set; }
            public bool IsEmpty { get; set; }
            public string EmptyMessage { get; set; } = "No completado";
            public bool IsBadge { get; set; }
            public string BadgeColor { get; set; } = "gray";

            public DetailField(string label, string value, bool isEmpty = false)
            {
                Label = label;
                Value = value;
                IsEmpty = isEmpty;
            }

            public static DetailField CreateBadge(string label, string value, string badgeColor = "purple")
            {
                return new DetailField(label, value)
                {
                    IsBadge = true,
                    BadgeColor = badgeColor
                };
            }

            public static DetailField CreateOptional(string label, string value)
            {
                return new DetailField(label, value, string.IsNullOrEmpty(value));
            }

            public string GetBadgeClasses()
            {
                return BadgeColor.ToLower() switch
                {
                    "blue" => "bg-blue-100 text-blue-800",
                    "purple" => "bg-purple-100 text-purple-800",
                    "green" => "bg-green-100 text-green-800",
                    "yellow" => "bg-yellow-100 text-yellow-800",
                    "red" => "bg-red-100 text-red-800",
                    _ => "bg-gray-100 text-gray-800"
                };
            }
        }
    }
