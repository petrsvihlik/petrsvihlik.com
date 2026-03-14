using Statiq.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetrSvihlik.Com.Models
{
    public class PagedContent<TContentModel>
    {
        private readonly IDocument _document;
        private readonly Lazy<PagedContent<TContentModel>> _previous;
        private readonly Lazy<PagedContent<TContentModel>> _next;

        public PagedContent(IReadOnlyList<TContentModel> items, IDocument document)
        {
            _document = document;
            Items = items;

            _previous = new Lazy<PagedContent<TContentModel>>(() =>
            {
                var otherDocument = document.GetDocument(Keys.Previous);
                return otherDocument != null ? new PagedContent<TContentModel>(
                    otherDocument.GetChildren().Select(c => c.Get<TContentModel>("ArticleModel")).Where(x => x != null).ToList(),
                    otherDocument) : null;
            });
            _next = new Lazy<PagedContent<TContentModel>>(() =>
            {
                var otherDocument = document.GetDocument(Keys.Next);
                return otherDocument != null ? new PagedContent<TContentModel>(
                    otherDocument.GetChildren().Select(c => c.Get<TContentModel>("ArticleModel")).Where(x => x != null).ToList(),
                    otherDocument) : null;
            });
        }

        public int Index => _document.GetInt(Keys.Index);
        public int TotalPages => _document.GetInt(Keys.TotalPages);
        public int TotalItems => _document.GetInt(Keys.TotalItems);
        public IReadOnlyList<TContentModel> Items { get; }
        public PagedContent<TContentModel> Previous => _previous.Value;
        public PagedContent<TContentModel> Next => _next.Value;
        public string Url => _document.GetLink();
    }
}
