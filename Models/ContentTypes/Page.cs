using PetrSvihlik.Com.Models;

namespace PetrSvihlik.Com.Models.ContentTypes
{
    public class Page : ITitleProvider
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }
        public string MetaDescription { get; set; }
        public bool ShowInNavigation { get; set; }

        string ITitleProvider.Title => Title;
        string ITitleProvider.ElementCodename => "title";
    }
}
