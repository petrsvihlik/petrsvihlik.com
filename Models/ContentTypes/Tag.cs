using PetrSvihlik.Com.Models;

namespace PetrSvihlik.Com.Models.ContentTypes
{
    public class Tag : ITitleProvider
    {
        public string Title { get; set; }
        public string Slug { get; set; }

        string ITitleProvider.Title => Title;
        string ITitleProvider.ElementCodename => "title";
    }
}
