using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Component
{
    public class HTMLBrowserSearchInfo
    {
        public int IdWebSite { get; set; }
        public string HomeUrl { get; set; }
        public string UrlSearch { get; set; }
        public string TextSearch { get; set; }
        public int PageNumber { get; set; }
        public int MaxShow { get; set; }
        public int PageTotalCount { get; set; }
        public int SiteTotalCount { get; set; }
        public int Order { get; set; }
        public int BlockNumber { get; set; }
        public bool InitPageNumberZero { get; set; }
        public TagSearchInfo TitleTag { get; set; }
        public TagSearchInfo VideoTag { get; set; }
        public TagSearchInfo ImageTag { get; set; }
        public TagSearchInfo RatingTag { get; set; }
        public TagSearchInfo ViewsTag { get; set; }
        public TagSearchInfo LikesTag { get; set; }
        public TagSearchInfo DurationTag { get; set; }
    }
}
