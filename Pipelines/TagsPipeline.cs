using PetrSvihlik.Com.Extensions;
using PetrSvihlik.Com.Models.ContentTypes;

namespace PetrSvihlik.Com.Pipelines
{
    public class TagsPipeline : GroupedArchivePipeline
    {
        public TagsPipeline()
            : base(nameof(Tag), "tag", slug => new Tag { Slug = slug, Title = slug.SlugToTitle() })
        {
        }
    }
}
