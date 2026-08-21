using PetrSvihlik.Com.Extensions;
using PetrSvihlik.Com.Models;
using PetrSvihlik.Com.Models.ContentTypes;
using PetrSvihlik.Com.Models.ViewModels;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;

namespace PetrSvihlik.Com.Pipelines
{
    public class HomePipeline : Pipeline
    {
        public HomePipeline()
        {
            Dependencies.AddRange(nameof(PostsPipeline), nameof(HomepagePipeline), nameof(SiteMetadataPipeline));
            ProcessModules = new ModuleList(
                new ReplaceDocuments(nameof(PostsPipeline)),
                new OrderDocuments(Config.FromDocument(doc => doc.Get<Article>(MetadataKeys.ArticleModel)?.PublishDate)).Descending(),
                new PaginateDocuments(GroupedArchivePipeline.PostsPerPage),
                new SetDestination(Config.FromDocument(GetDestination)),
                new MergeContent(new ReadFiles("_Index.cshtml")),
                new RenderRazor()
                    .WithModel(Config.FromDocument((document, context) =>
                        new HomeViewModel(
                            document.AsPagedContent<Article>(),
                            context.CreateSidebar(isIndex: true, activeMenuItem: "/"))
                        {
                            // full archive (newest-first) so the homepage filter
                            // can search every post, not just the current page
                            AllArticles = context.GetArticles()
                        }))
            );

            OutputModules = new ModuleList
            {
                new WriteFiles(),
            };
        }

        private static NormalizedPath GetDestination(IDocument document)
        {
            var index = document.GetInt(Keys.Index);
            return new NormalizedPath($"{(index > 1 ? $"page/{index}/" : "")}index.html");
        }
    }
}
