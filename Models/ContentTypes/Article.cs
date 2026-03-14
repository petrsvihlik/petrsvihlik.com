using System;
using System.Collections.Generic;
using PetrSvihlik.Com.Models;

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

        string ITitleProvider.Title => Title;
        string ITitleProvider.ElementCodename => "title";
    }
}
