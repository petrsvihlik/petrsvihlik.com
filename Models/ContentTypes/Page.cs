namespace PetrSvihlik.Com.Models.ContentTypes
{
    public class Page : ITitleProvider
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }
        public string MetaDescription { get; set; }

        /// <summary>Rendered, but kept out of the sitemap and noindexed (e.g. test pages).</summary>
        public bool Unlisted { get; set; }

        /// <summary>Show the giscus comment thread under the content.</summary>
        public bool Comments { get; set; }

        /// <summary>Show the newsletter signup form under the content.</summary>
        public bool Newsletter { get; set; }
    }
}
