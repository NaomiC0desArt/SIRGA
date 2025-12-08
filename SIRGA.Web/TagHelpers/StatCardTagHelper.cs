using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SIRGA.Web.TagHelpers
{
    /// Tag Helper para crear tarjetas de estadísticas reutilizables
    /// Uso: <stat-card title="Total Estudiantes" value="20" color="blue" icon="users" />
    /// 

    [HtmlTargetElement("stat-card")]
    public class StatCardTagHelper : TagHelper
    {
        public string Title { get; set; } = "";
        public string Value { get; set; } = "0";
        public string Color { get; set; } = "blue"; // blue, green, purple, orange, indigo, teal, pink
        public string Icon { get; set; } = "users";

        private readonly Dictionary<string, string> ColorSchemes = new()
        {
            { "blue", "from-blue-500 to-blue-600|text-blue-100" },
            { "green", "from-green-500 to-green-600|text-green-100" },
            { "purple", "from-purple-500 to-purple-600|text-purple-100" },
            { "orange", "from-orange-500 to-orange-600|text-orange-100" },
            { "indigo", "from-indigo-500 to-indigo-600|text-indigo-100" },
            { "teal", "from-teal-500 to-teal-600|text-teal-100" },
            { "pink", "from-pink-500 to-pink-600|text-pink-100" }
        };

        private readonly Dictionary<string, string> IconPaths = new()
        {
            { "users", "M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" },
            { "check", "M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" },
            { "building", "M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" },
            { "clipboard", "M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" },
            { "calendar", "M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" },
            { "chart", "M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" }
        };

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "bg-gradient-to-br " + GetColorGradient() + " rounded-xl shadow-lg p-6 text-white transform hover:scale-105 transition-all duration-200");

            var textColor = GetTextColor();
            var iconPath = IconPaths.ContainsKey(Icon) ? IconPaths[Icon] : IconPaths["users"];

            output.Content.SetHtmlContent($@"
                <div class='flex items-center justify-between'>
                    <div>
                        <p class='{textColor} text-sm font-medium'>{Title}</p>
                        <p class='text-3xl font-bold mt-2'>{Value}</p>
                    </div>
                    <div class='bg-white bg-opacity-20 rounded-lg p-3'>
                        <svg class='w-8 h-8' fill='none' stroke='currentColor' viewBox='0 0 24 24'>
                            <path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='{iconPath}'></path>
                        </svg>
                    </div>
                </div>
            ");
        }

        private string GetColorGradient()
        {
            return ColorSchemes.ContainsKey(Color) ? ColorSchemes[Color].Split('|')[0] : ColorSchemes["blue"].Split('|')[0];
        }

        private string GetTextColor()
        {
            return ColorSchemes.ContainsKey(Color) ? ColorSchemes[Color].Split('|')[1] : ColorSchemes["blue"].Split('|')[1];
        }
    }
}
