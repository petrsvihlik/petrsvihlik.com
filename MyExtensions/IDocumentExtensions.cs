using PetrSvihlik.Com.Models;
using Statiq.Common;
using System.Linq;

namespace PetrSvihlik.Com.Extensions
{
    public static class IDocumentExtensions
    {
        public static PagedContent<TModel> AsPagedContent<TModel>(this IDocument document)
        {
            var items = document.GetChildren()
                .Select(c => c.Get<TModel>("ArticleModel"))
                .Where(x => x != null)
                .ToList();
            return new PagedContent<TModel>(items, document);
        }
    }
}
