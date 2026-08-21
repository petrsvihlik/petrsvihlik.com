using PetrSvihlik.Com.Models.ContentTypes;

namespace PetrSvihlik.Com.Generation
{
    /// <summary>Site-wide state every rendered page needs.</summary>
    public sealed record PageContext(SiteMetadata Site, SiteLinks Links, string TagManagerId)
    {
        public Author Author => Site.SiteAuthor;
    }
}
