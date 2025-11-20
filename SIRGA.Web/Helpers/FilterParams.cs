namespace SIRGA.Web.Helpers
{
    public class FilterParams
    {
        public string SearchTerm { get; set; } = "";
        public string Status { get; set; } = "";
        public string SortBy { get; set; } = "";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public Dictionary<string, string> ToQueryString()
        {
            return new Dictionary<string, string>
            {
                { "searchTerm", SearchTerm },
                { "status", Status },
                { "sortBy", SortBy },
                { "pageNumber", PageNumber.ToString() },
                { "pageSize", PageSize.ToString() }
            };
        }
    }
}
