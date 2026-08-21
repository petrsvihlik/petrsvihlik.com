using PetrSvihlik.Com.Models.ContentTypes;

namespace PetrSvihlik.Com.Generation
{
    public enum JsonLdType
    {
        None,
        WebSite,
        BlogPosting,
    }

    /// <summary>Per-page SEO data rendered into head by SeoHead.</summary>
    public sealed record PageSeo
    {
        /// <summary>Browser/OG title; null falls back to the site title.</summary>
        public string Title { get; init; }

        /// <summary>Meta/OG description; null falls back to the site subtitle.</summary>
        public string Description { get; init; }

        /// <summary>This page's site-relative URL (LinkRoot included), e.g. "/posts/slug".</summary>
        public string RootedUrl { get; init; }

        /// <summary>External canonical URL for content originally published elsewhere.</summary>
        public string ExternalCanonical { get; init; }

        /// <summary>Set on post pages: drives article OG tags and BlogPosting JSON-LD.</summary>
        public Article Article { get; init; }

        /// <summary>Structured data block to emit (requires a configured Host).</summary>
        public JsonLdType JsonLd { get; init; }

        /// <summary>Ask crawlers not to index this page (thin archives, 404).</summary>
        public bool NoIndex { get; init; }
    }
}
