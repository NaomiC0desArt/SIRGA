using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SIRGA.Web.TagHelpers
{
    /// Tag Helper para crear tarjetas de navegación reutilizables
    /// Uso: <nav-card title="Gestión de Estudiantes" description="Crear y editar estudiantes" 
    ///               controller="Admin" action="Estudiantes" color="blue" icon="users" />
    [HtmlTargetElement("nav-card")]
    public class NavCardTagHelper : TagHelper
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "";
        public string Color { get; set; } = "blue";
        public string Icon { get; set; } = "users";

        private readonly Dictionary<string, string> ColorSchemes = new()
        {
            { "blue", "from-blue-50 to-blue-100|hover:from-blue-100 hover:to-blue-200|border-blue-200|bg-blue-500|group-hover:text-blue-700" },
            { "green", "from-green-50 to-green-100|hover:from-green-100 hover:to-green-200|border-green-200|bg-green-500|group-hover:text-green-700" },
            { "purple", "from-purple-50 to-purple-100|hover:from-purple-100 hover:to-purple-200|border-purple-200|bg-purple-500|group-hover:text-purple-700" },
            { "orange", "from-orange-50 to-orange-100|hover:from-orange-100 hover:to-orange-200|border-orange-200|bg-orange-500|group-hover:text-orange-700" },
            { "indigo", "from-indigo-50 to-indigo-100|hover:from-indigo-100 hover:to-indigo-200|border-indigo-200|bg-indigo-500|group-hover:text-indigo-700" },
            { "teal", "from-teal-50 to-teal-100|hover:from-teal-100 hover:to-teal-200|border-teal-200|bg-teal-500|group-hover:text-teal-700" },
            { "pink", "from-pink-50 to-pink-100|hover:from-pink-100 hover:to-pink-200|border-pink-200|bg-pink-500|group-hover:text-pink-700" }
        };

        private readonly Dictionary<string, string> IconPaths = new()
        {
            { "users", "M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" },
            { "book", "M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" },
            { "building", "M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" },
            { "calendar", "M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" },
            { "clipboard", "M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01" },
            { "document", "M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" },
            { "smile", "M14.828 14.828a4 4 0 01-5.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" },
            { "check", "M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" },
            { "teacher", "M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" },
            { "presentation", "M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" }
        };

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var colorParts = GetColorParts();
            var iconPath = IconPaths.ContainsKey(Icon) ? IconPaths[Icon] : IconPaths["users"];

            output.TagName = "a";
            output.Attributes.SetAttribute("asp-controller", Controller);
            output.Attributes.SetAttribute("asp-action", Action);
            output.Attributes.SetAttribute("class", $"group bg-gradient-to-r {colorParts[0]} {colorParts[1]} rounded-xl p-6 border {colorParts[2]} transition-all duration-200 hover:shadow-lg");

            output.Content.SetHtmlContent($@"
                <div class='flex items-center'>
                    <div class='{colorParts[3]} rounded-lg p-4 mr-4'>
                        <svg class='w-8 h-8 text-white' fill='none' stroke='currentColor' viewBox='0 0 24 24'>
                            <path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='{iconPath}'></path>
                        </svg>
                    </div>
                    <div>
                        <h3 class='text-xl font-semibold text-gray-900 {colorParts[4]}'>
                            {Title}
                        </h3>
                        <p class='text-sm text-gray-600 mt-1'>{Description}</p>
                    </div>
                </div>
            ");
        }

        private string[] GetColorParts()
        {
            if (ColorSchemes.ContainsKey(Color))
            {
                return ColorSchemes[Color].Split('|');
            }
            return ColorSchemes["blue"].Split('|');
        }
    }
