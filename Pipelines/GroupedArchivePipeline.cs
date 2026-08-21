using PetrSvihlik.Com.Extensions;
using PetrSvihlik.Com.Models;
using PetrSvihlik.Com.Models.ContentTypes;
using PetrSvihlik.Com.Models.ViewModels;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using System;
using System.Collections.Generic;

namespace PetrSvihlik.Com.Pipelines
{
    /// <summary>
    /// Renders a paginated archive of posts for every value of a grouping key
    /// (e.g. one archive per tag or per category) at <c>{pathPrefix}/{slug}[/{page}]/index.html</c>.
    /// </summary>
    public abstract class GroupedArchivePipeline : Pipeline
    {
        public const int PostsPerPage = 4;

        protected GroupedArchivePipeline(string groupKey, string pathPrefix, Func<string, TaxonomyTerm> createTerm)
        {
            Dependencies.AddRange(nameof(PostsPipeline), nameof(HomepagePipeline), nameof(SiteMetadataPipeline));

            ProcessModules = new ModuleList
            {
                new ReplaceDocuments(nameof(PostsPipeline)),
                new GroupDocuments(groupKey),
                new SetMetadata(MetadataKeys.SelectedGroup, Config.FromDocument(doc =>
                    createTerm(doc.GetString(Keys.GroupKey) ?? ""))),
                new ForEachDocument
                {
                    new ExecuteConfig(Config.FromDocument(groupDoc => new ModuleList
                    {
                        new ReplaceDocuments(Config.FromDocument<IEnumerable<IDocument>>(doc => doc.GetChildren())),
                        new OrderDocuments(Config.FromDocument(doc => doc.Get<Article>(MetadataKeys.ArticleModel)?.PublishDate)).Descending(),
                        new PaginateDocuments(PostsPerPage),
                        new MergeContent(new ReadFiles("_Index.cshtml")),
                        new RenderRazor()
                            .WithModel(Config.FromDocument((document, context) =>
                                new HomeViewModel(
                                    document.AsPagedContent<Article>(),
                                    context.CreateSidebar(),
                                    groupDoc.Get<TaxonomyTerm>(MetadataKeys.SelectedGroup)))),
                        new SetDestination(Config.FromDocument(doc => GetDestination(doc, groupDoc, pathPrefix))),
                    }))
                }
            };

            OutputModules = new ModuleList
            {
                new WriteFiles(),
            };
        }

        private static NormalizedPath GetDestination(IDocument page, IDocument group, string pathPrefix)
        {
            var index = page.GetInt(Keys.Index);
            var slug = group.Get<TaxonomyTerm>(MetadataKeys.SelectedGroup).Slug;
            return new NormalizedPath($"{pathPrefix}/{slug}/{(index > 1 ? $"{index}/" : "")}index.html");
        }
    }
}
