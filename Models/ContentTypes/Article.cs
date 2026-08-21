using System;
using System.Collections.Generic;

namespace PetrSvihlik.Com.Models.ContentTypes
{
    public class Article : ITitleProvider
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public DateTime? PublishDate { get; set; }
        public string CanonicalUrl { get; set; }
        public string ContentHtml { get; set; }
        public Category SelectedCategory { get; set; }
        public List<Tag> TagObjects { get; set; } = new();
        public Author ArticleAuthor { get; set; }

        /// <summary>Show the giscus comment thread under the post.</summary>
        public bool Comments { get; set; }

        /// <summary>Excluded from production builds, archives, feeds, and the sitemap.</summary>
        public bool Draft { get; set; }
    }
}
