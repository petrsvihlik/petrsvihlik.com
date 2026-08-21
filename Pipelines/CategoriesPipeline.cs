using PetrSvihlik.Com.Extensions;
using PetrSvihlik.Com.Models.ContentTypes;

namespace PetrSvihlik.Com.Pipelines
{
    public class CategoriesPipeline : GroupedArchivePipeline
    {
        public CategoriesPipeline()
            : base(nameof(Category), "category", slug => new Category { Slug = slug, Title = slug.SlugToTitle() })
        {
        }
    }
}
