using Kontent.Ai.Delivery.Abstractions;
using PetrSvihlik.Com.Models.ContentTypes;
using Kontent.Statiq;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using System.Linq;
using PetrSvihlik.Com.Models.ViewModels;
using PetrSvihlik.Com.Extensions;

namespace PetrSvihlik.Com.Pipelines
{
    public class HomePipeline : Pipeline
    {
        public HomePipeline(IDeliveryClient deliveryClient)
        {
            Dependencies.AddRange(nameof(PostsPipeline), nameof(HomepagePipeline), nameof(SiteMetadataPipeline));
            ProcessModules = new ModuleList(
                // pull documents from other pipelines
                new ReplaceDocuments(nameof(PostsPipeline)),
                new PaginateDocuments(4),
                new SetDestination(Config.FromDocument((doc, ctx) => Filename(doc))),
                new MergeContent(new ReadFiles("Index.cshtml")),
                new RenderRazor()
                    .WithModel(Config.FromDocument((document, context) =>
                    {
                        var model = new HomeViewModel(document.AsPagedKontent<Article>(),
                           new SidebarViewModel(
                               context.Outputs.FromPipeline(nameof(HomepagePipeline)).Select(x => x.AsKontent<Homepage>()).FirstOrDefault(),
                               context.Outputs.FromPipeline(nameof(SiteMetadataPipeline)).Select(x => x.AsKontent<SiteMetadata>()).FirstOrDefault(),
                               true, "/"));
                        return model;
                    }
                    )),
                new KontentImageProcessor()
            );

            OutputModules = new ModuleList {
                new WriteFiles(),
            };
        }

        private static NormalizedPath Filename(IDocument document)
        {
            var index = document.GetInt(Keys.Index);

            return new NormalizedPath($"{(index > 1 ? $"page/{index}/" : "")}index.html");
        }
    }
}
