using PetrSvihlik.Com.Models;
using PetrSvihlik.Com.Models.ContentTypes;
using PetrSvihlik.Com.Models.ViewModels;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using System.Collections.Generic;
using System.Linq;

namespace PetrSvihlik.Com.Pipelines
{
    public class TagsPipeline : Pipeline
    {
        public TagsPipeline()
        {
            Dependencies.AddRange(nameof(PostsPipeline), nameof(HomepagePipeline), nameof(SiteMetadataPipeline));

            ProcessModules = new ModuleList
            {
                new ReplaceDocuments(nameof(PostsPipeline)),
                new GroupDocuments(nameof(Tag)),
                new SetMetadata("SelectedTag", Config.FromDocument(doc =>
                {
                    var slug = doc.GetString(Keys.GroupKey) ?? "";
                    return new Tag { Slug = slug, Title = PostsPipeline.SlugToTitle(slug) };
                })),
                new ForEachDocument
                {
                    new ExecuteConfig(Config.FromDocument(groupDoc => new ModuleList
                    {
                        new ReplaceDocuments(Config.FromDocument<IEnumerable<IDocument>>(doc => doc.GetChildren())),
                        new PaginateDocuments(4),
                        new MergeContent(new ReadFiles("_Index.cshtml")),
                        new RenderRazor()
                            .WithModel(Config.FromDocument((document, context) =>
                            {
                                var tag = groupDoc.Get<Tag>("SelectedTag");
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
                                    new SidebarViewModel(homepage, metadata, false, null), tag);
                            })),
                        new SetDestination(Config.FromDocument((doc, ctx) => Filename(doc, groupDoc))),
                    }))
                }
            };

            OutputModules = new ModuleList
            {
                new WriteFiles(),
            };
        }

        private static NormalizedPath Filename(IDocument page, IDocument group)
        {
            var index = page.GetInt(Keys.Index);
            var tag = group.Get<Tag>("SelectedTag").Slug;
            return new NormalizedPath($"tag/{tag}/{(index > 1 ? $"{index}/" : "")}index.html");
        }
    }
}
