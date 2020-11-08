using PetrSvihlik.Com.Models;
using Statiq.Common;

namespace PetrSvihlik.Com.Extensions
{
    public static class IDocumentExtensions
    {
        public static PagedContent<TModel> AsPagedKontent<TModel>(this IDocument document)
        {
            return new PagedContent<TModel>(document);
        }
    }
}
