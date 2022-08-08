using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Urls.Delivery.QueryParameters;
using Kontent.Ai.Urls.Delivery.QueryParameters.Filters;
using PetrSvihlik.Com.Models.ContentTypes;
using Kontent.Statiq;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using System.Linq;
using PetrSvihlik.Com.Models.ViewModels;

namespace PetrSvihlik.Com.Pipelines
{
    public class PagesPipeline : Pipeline
    {
        public PagesPipeline(IDeliveryClient deliveryClient)
        {
            Dependencies.AddRange(nameof(HomepagePipeline), nameof(SiteMetadataPipeline));
            InputModules = new ModuleList{
                new Kontent<Page>(deliveryClient)
                    .WithQuery(new IncludeTotalCountParameter(), new NotEmptyFilter("elements.body")),
                new SetDestination(Config.FromDocument((doc, ctx)  => GetPath(doc))),
            };

            ProcessModules = new ModuleList {
                new MergeContent(new ReadFiles(patterns: "Index.cshtml") ),
                new RenderRazor()
                 .WithModel(Config.FromDocument((document, context) =>
                 {
                    var menuItem = document.AsKontent<Page>();
                    var model = new HomeViewModel(menuItem,
                                    new SidebarViewModel(
                                    context.Outputs.FromPipeline(nameof(HomepagePipeline)).Select(x => x.AsKontent<Homepage>()).FirstOrDefault(),
                                    context.Outputs.FromPipeline(nameof(SiteMetadataPipeline)).Select(x => x.AsKontent<SiteMetadata>()).FirstOrDefault(),
                                    false, menuItem.Url));
                    return model;
                 }
                 ))/*,
                new KontentImageProcessor()*/
            };

            OutputModules = new ModuleList {
                new WriteFiles(),
            };
        }

        private static NormalizedPath GetPath(IDocument doc)
        {
            Page page = doc.AsKontent<Page>();
            string path;
            if (page.Url == "404")
            {
                path = $"{page.Url}.html";
            }
            else
            {
                path = $"pages/{page.Url}/index.html";
            }
            return new NormalizedPath(path);
        }
    }
}