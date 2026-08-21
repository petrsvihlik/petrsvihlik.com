namespace PetrSvihlik.Com.Models.ContentTypes
{
    /// <summary>
    /// A term posts can be grouped by (category, tag): a slug used in URLs
    /// plus a human-readable title derived from it.
    /// </summary>
    public abstract class TaxonomyTerm : ITitleProvider
    {
        public string Title { get; set; }
        public string Slug { get; set; }
    }
}
