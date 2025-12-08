using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SIRGA.Web.TagHelpers
{
    /// Tag Helper para contenedores de sección con estilos consistentes
    /// Uso: <section-container><content>...</content></section-container>

    [HtmlTargetElement("section-container")]
    public class SectionContainerTagHelper : TagHelper
    {
        public string MarginBottom { get; set; } = "mb-8"; // mb-6, mb-8, mb-0

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", $"bg-white rounded-2xl shadow-xl p-6 sm:p-8 border border-gray-100 {MarginBottom}");

            var childContent = await output.GetChildContentAsync();
            output.Content.SetHtmlContent(childContent);
        }
    }

    /// Tag Helper para el contenedor principal de la página
    /// Uso: <page-container><content>...</content></page-container>

    [HtmlTargetElement("page-container")]
    public class PageContainerTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "min-h-screen bg-gray-50 px-3 sm:px-4 lg:px-8 pt-8 pb-12");

            var childContent = await output.GetChildContentAsync();
            output.Content.SetHtmlContent($@"
                <div class='max-w-7xl mx-auto'>
                    {childContent.GetContent()}
                </div>
            ");
        }
    }

    /// Tag Helper para grid de estadísticas o tarjetas
    /// Uso: <stats-grid><stat-card.../><stat-card.../></stats-grid>
    [HtmlTargetElement("stats-grid")]
    public class StatsGridTagHelper : TagHelper
    {
        public string Columns { get; set; } = "4"; // 2, 3, 4

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            var gridCols = Columns switch
            {
                "2" => "md:grid-cols-2",
                "3" => "md:grid-cols-2 lg:grid-cols-3",
                "4" => "md:grid-cols-2 lg:grid-cols-4",
                _ => "md:grid-cols-2 lg:grid-cols-4"
            };

            output.Attributes.SetAttribute("class", $"grid grid-cols-1 {gridCols} gap-6 mb-8");

            var childContent = await output.GetChildContentAsync();
            output.Content.SetHtmlContent(childContent);
        }
    }

    /// Tag Helper para grid de navegación
    /// Uso: <nav-grid><nav-card.../><nav-card.../></nav-grid>
    [HtmlTargetElement("nav-grid")]
    public class NavGridTagHelper : TagHelper
    {
        public string Columns { get; set; } = "2"; // 1, 2, 3

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            var gridCols = Columns switch
            {
                "1" => "grid-cols-1",
                "2" => "md:grid-cols-2",
                "3" => "md:grid-cols-2 lg:grid-cols-3",
                _ => "md:grid-cols-2"
            };

            output.Attributes.SetAttribute("class", $"grid grid-cols-1 {gridCols} gap-6");

            var childContent = await output.GetChildContentAsync();
            output.Content.SetHtmlContent(childContent);
        }
    }
}
