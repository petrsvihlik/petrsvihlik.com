using PetrSvihlik.Com.Models;
using PetrSvihlik.Com.Models.ContentTypes;
using PetrSvihlik.Com.Models.ViewModels;
using Statiq.Common;
using Statiq.Core;
using Statiq.Markdown;
using Statiq.Razor;
using Statiq.Yaml;

namespace PetrSvihlik.Com.Pipelines
{
    public class PagesPipeline : Pipeline
    {
        public PagesPipeline()
        {
            Dependencies.AddRange(nameof(HomepagePipeline), nameof(SiteMetadataPipeline));
            InputModules = new ModuleList
            {
                new ReadFiles("pages/*.md"),
                new ExtractFrontMatter(new ParseYaml()),
                new RenderMarkdown().UseExtensions(),
                new SetMetadata(MetadataKeys.RenderedBody, Config.FromDocument(async doc => await doc.GetContentStringAsync())),
                new SetDestination(Config.FromDocument(GetDestination)),
            };

            ProcessModules = new ModuleList
            {
                new MergeContent(new ReadFiles("_Index.cshtml")),
                new RenderRazor()
                    .WithModel(Config.FromDocument((document, context) =>
                    {
                        var page = new Page
                        {
                            Title = document.GetString("title"),
                            Url = GetSlug(document),
                            Body = document.GetString(MetadataKeys.RenderedBody),
                            MetaDescription = document.GetString("description"),
                            ShowInNavigation = document.GetBool("show_in_navigation"),
                        };
                        return new HomeViewModel(page, context.CreateSidebar(activeMenuItem: page.Url));
                    }))
            };

            OutputModules = new ModuleList
            {
                new WriteFiles(),
            };
        }

        private static string GetSlug(IDocument doc) =>
            doc.GetString("slug") ?? doc.Source.FileNameWithoutExtension.FullPath;

        private static NormalizedPath GetDestination(IDocument doc)
        {
            var slug = GetSlug(doc);
            return slug == "404"
                ? new NormalizedPath("404.html")
                : new NormalizedPath($"pages/{slug}/index.html");
        }
    }
}
