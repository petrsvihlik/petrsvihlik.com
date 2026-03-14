using PetrSvihlik.Com.Models.ContentTypes;
using PetrSvihlik.Com.Models.ViewModels;
using Statiq.Common;
using Statiq.Core;
using Statiq.Markdown;
using Statiq.Razor;
using Statiq.Yaml;
using System.Linq;

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
                new RenderMarkdown(),
                new SetMetadata("RenderedBody", Config.FromDocument(async doc => await doc.GetContentStringAsync())),
                new SetDestination(Config.FromDocument(doc => GetPath(doc))),
            };

            ProcessModules = new ModuleList
            {
                new MergeContent(new ReadFiles("_Index.cshtml")),
                new RenderRazor()
                    .WithModel(Config.FromDocument((document, context) =>
                    {
                        var slug = document.GetString("slug") ?? document.Source.FileNameWithoutExtension.FullPath;
                        var page = new Page
                        {
                            Title = document.GetString("title"),
                            Url = slug,
                            Body = document.GetString("RenderedBody"),
                            MetaDescription = document.GetString("description"),
                            ShowInNavigation = document.GetBool("show_in_navigation"),
                        };
                        var model = new HomeViewModel(page,
                            new SidebarViewModel(
                                context.Outputs.FromPipeline(nameof(HomepagePipeline)).Select(x => x.Get<Homepage>("Homepage")).FirstOrDefault(),
                                context.Outputs.FromPipeline(nameof(SiteMetadataPipeline)).Select(x => x.Get<SiteMetadata>("SiteMetadata")).FirstOrDefault(),
                                false, page.Url));
                        return model;
                    }))
            };

            OutputModules = new ModuleList
            {
                new WriteFiles(),
            };
        }

        private static NormalizedPath GetPath(IDocument doc)
        {
            var slug = doc.GetString("slug") ?? doc.Source.FileNameWithoutExtension.FullPath;
            return slug == "404"
                ? new NormalizedPath("404.html")
                : new NormalizedPath($"pages/{slug}/index.html");
        }
    }
}
