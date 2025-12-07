namespace SIRGA.Web.Models.Components
{
    public class StatCardViewModel
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string Color { get; set; } = "blue";
        public string Icon { get; set; } = "users";
    }
    public class NavCardViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Color { get; set; } = "blue";
        public string Icon { get; set; } = "users";
    }

    public class PageHeaderViewModel
    {
        public string Title { get; set; }
        public string Username { get; set; }
        public string Icon { get; set; } = "shield";
        public string Color { get; set; } = "blue";
        public bool ShowDate { get; set; } = false;
    }

    public class SectionHeaderViewModel
    {
        public string Title { get; set; }
        public string Icon { get; set; } = "menu";
        public string Color { get; set; } = "blue";
        public string Subtitle { get; set; }
    }
}
