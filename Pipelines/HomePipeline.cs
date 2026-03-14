using PetrSvihlik.Com.Models;
using PetrSvihlik.Com.Models.ContentTypes;
using PetrSvihlik.Com.Models.ViewModels;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using System.Linq;

namespace PetrSvihlik.Com.Pipelines
{
    public class HomePipeline : Pipeline
    {
        public HomePipeline()
        {
            Dependencies.AddRange(nameof(PostsPipeline), nameof(HomepagePipeline), nameof(SiteMetadataPipeline));
            ProcessModules = new ModuleList(
                new ReplaceDocuments(nameof(PostsPipeline)),
                new PaginateDocuments(4),
                new SetDestination(Config.FromDocument(doc => Filename(doc))),
                new MergeContent(new ReadFiles("_Index.cshtml")),
                new RenderRazor()
                    .WithModel(Config.FromDocument((document, context) =>
                    {
                        var articles = document.GetChildren()
                            .Select(d => d.Get<Article>("ArticleModel"))
                            .Where(a => a != null)
                            .ToList();
                        var paged = new PagedContent<Article>(articles, document);
                        var metadata = context.Outputs.FromPipeline(nameof(SiteMetadataPipeline))
                            .Select(x => x.Get<SiteMetadata>("SiteMetadata")).FirstOrDefault();
                        var homepage = context.Outputs.FromPipeline(nameof(HomepagePipeline))
                            .Select(x => x.Get<Homepage>("Homepage")).FirstOrDefault();
                        return new HomeViewModel(paged,
                            new SidebarViewModel(homepage, metadata, true, "/"));
                    }))
            );

            OutputModules = new ModuleList
            {
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
