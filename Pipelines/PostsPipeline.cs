using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Urls.Delivery.QueryParameters;
using PetrSvihlik.Com.Models.ContentTypes;
using Kontent.Statiq;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using System.Linq;
using PetrSvihlik.Com.Models.ViewModels;

namespace PetrSvihlik.Com.Pipelines
{
    public class PostsPipeline : Pipeline
    {
        public PostsPipeline(IDeliveryClient deliveryClient)
        {
            Dependencies.AddRange(nameof(SiteMetadataPipeline));
            InputModules = new ModuleList{
                new Kontent<Article>(deliveryClient)
                    .WithQuery(new DepthParameter(1), new IncludeTotalCountParameter(), new OrderParameter($"elements.{Article.PublishDateCodename}", SortOrder.Descending)),
                new SetMetadata(nameof(Category), Config.FromDocument((doc, ctx) =>
                {
                    // Add category (useful for grouping)
                    return doc.AsKontent<Article>().SelectedCategory.System.Codename;
                })),
                new SetMetadata(nameof(Article.SelectedCategory), Config.FromDocument((doc, ctx) =>
                {
                    // Add some extra metadata to be used later for creating filenames
                    return doc.AsKontent<Article>().SelectedCategory;
                })),
                new SetMetadata(nameof(Tag), Config.FromDocument((doc, ctx) =>
                {
                    // Add tag (useful for grouping)
                    return doc.AsKontent<Article>().TagObjects.Select(t=>t.System.Codename);
                })),
                new SetMetadata(nameof(Article.TagObjects), Config.FromDocument((doc, ctx) =>
                {
                    // Add some extra metadata to be used later for creating filenames
                    return doc.AsKontent<Article>().TagObjects;
                })),
                new SetDestination(Config.FromDocument((doc, ctx)  => new NormalizedPath($"posts/{doc.AsKontent<Article>().Slug}.html" ))),
            };

            ProcessModules = new ModuleList {
                new MergeContent(new ReadFiles(patterns: "Post.cshtml") ),
                new RenderRazor()
                 .WithModel(Config.FromDocument((document, context) =>
                 new PostViewModel(document.AsKontent<Article>(),
                               context.Outputs.FromPipeline(nameof(SiteMetadataPipeline)).Select(x => x.AsKontent<SiteMetadata>()).FirstOrDefault()))),
                new KontentImageProcessor()
            };

            OutputModules = new ModuleList {
                new WriteFiles(),
            };
        }
    }
}