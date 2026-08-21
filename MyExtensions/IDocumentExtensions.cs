using PetrSvihlik.Com.Models;
using Statiq.Common;
using System.Linq;

namespace PetrSvihlik.Com.Extensions
{
    public static class IDocumentExtensions
    {
        /// <summary>
        /// Wraps a paginated document (produced by <c>PaginateDocuments</c>) whose children
        /// carry <see cref="MetadataKeys.ArticleModel"/> metadata into a <see cref="PagedContent{TModel}"/>.
        /// </summary>
        public static PagedContent<TModel> AsPagedContent<TModel>(this IDocument document)
        {
            var items = document.GetChildren()
                .Select(c => c.Get<TModel>(MetadataKeys.ArticleModel))
                .Where(x => x != null)
                .ToList();
            return new PagedContent<TModel>(items, document);
        }
    }
}
