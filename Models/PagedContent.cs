using System;
using System.Collections.Generic;
using System.Linq;

namespace PetrSvihlik.Com.Models
{
    /// <summary>One page of a paginated list, linked to its neighbors.</summary>
    public class PagedContent<TContentModel>
    {
        /// <summary>1-based page index.</summary>
        public int Index { get; init; }
        public int TotalPages { get; init; }
        public int TotalItems { get; init; }
        public IReadOnlyList<TContentModel> Items { get; init; }
        public string Url { get; init; }
        public PagedContent<TContentModel> Previous { get; set; }
        public PagedContent<TContentModel> Next { get; set; }

        /// <summary>Splits items into linked pages and assigns each page its URL.</summary>
        public static List<PagedContent<TContentModel>> Paginate(
            IReadOnlyList<TContentModel> items, int pageSize, Func<int, string> getUrl)
        {
            var pages = new List<PagedContent<TContentModel>>();
            var totalPages = (items.Count + pageSize - 1) / pageSize;
            for (var i = 0; i < totalPages; i++)
            {
                pages.Add(new PagedContent<TContentModel>
                {
                    Index = i + 1,
                    TotalPages = totalPages,
                    TotalItems = items.Count,
                    Items = items.Skip(i * pageSize).Take(pageSize).ToList(),
                    Url = getUrl(i + 1),
                });
            }

            for (var i = 0; i < pages.Count; i++)
            {
                pages[i].Previous = i > 0 ? pages[i - 1] : null;
                pages[i].Next = i < pages.Count - 1 ? pages[i + 1] : null;
            }

            return pages;
        }
    }
}
