using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetrSvihlik.Com.Components;
using PetrSvihlik.Com.Extensions;
using PetrSvihlik.Com.Models;
using PetrSvihlik.Com.Models.ContentTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PetrSvihlik.Com.Generation
{
    /// <summary>
    /// Generates the whole site from input/ into output/: renders Razor components to HTML,
    /// writes feeds and the sitemap, and copies static assets.
    /// </summary>
    public sealed class SiteBuilder
    {
        public const int PostsPerPage = 4;

        private readonly string _inputPath;
        private readonly string _outputPath;
        private readonly PageContext _ctx;
        private readonly SiteLinks _links;

        public SiteBuilder(string rootPath, IConfiguration configuration)
        {
            _inputPath = Path.Combine(rootPath, "input");
            _outputPath = Path.Combine(rootPath, "output");
            _links = new SiteLinks(configuration["LinkRoot"], configuration["Host"]);
            _ctx = new PageContext(CreateSiteMetadata(), _links, configuration["TagManagerId"]);
        }

        private static SiteMetadata CreateSiteMetadata() => new()
        {
            Title = "Petr Švihlík",
            Subtitle = "Posts about DevRel, DX, OS, and .NET.",
            SiteAuthor = new Author
            {
                Name = "Petr Švihlík",
                Contacts = new List<Contact>
                {
                    new() { Name = "GitHub", Url = "https://github.com/petrsvihlik" },
                    new() { Name = "Stack Overflow", Url = "https://stackoverflow.com/users/1332034/rocky" },
                    new() { Name = "Dev.to", Url = "https://dev.to/petrsvihlik" },
                    new() { Name = "Medium", Url = "https://medium.com/@PetrSvihlik" },
                    new() { Name = "LinkedIn", Url = "https://www.linkedin.com/in/svihlik/" },
                    new() { Name = "RSS", Url = "/feed.rss" },
                }
            }
        };

        public async Task BuildAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            if (Directory.Exists(_outputPath))
            {
                Directory.Delete(_outputPath, recursive: true);
            }

            var articles = LoadArticles();
            var pages = LoadPages();
            var projects = LoadProjects();

            CopyStaticAssets();

            var services = new ServiceCollection();
            services.AddLogging();
            await using var serviceProvider = services.BuildServiceProvider();
            await using var renderer = new HtmlRenderer(serviceProvider, serviceProvider.GetRequiredService<ILoggerFactory>());

            Task<string> RenderAsync<TComponent>(Dictionary<string, object> parameters) where TComponent : IComponent =>
                renderer.Dispatcher.InvokeAsync(async () =>
                {
                    var output = await renderer.RenderComponentAsync<TComponent>(ParameterView.FromDictionary(parameters));
                    return output.ToHtmlString();
                });

            var newestFirst = articles.OrderByDescending(a => a.PublishDate).ToList();

            // posts (and their <article> bodies, reused by the feeds)
            var articleBodies = new Dictionary<string, string>();
            foreach (var article in articles)
            {
                articleBodies[article.Slug] = await RenderAsync<PostArticle>(new() { ["Ctx"] = _ctx, ["Article"] = article });
                var html = await RenderAsync<PostPage>(new() { ["Ctx"] = _ctx, ["Article"] = article });
                WriteFile($"posts/{article.Slug}.html", html);
            }

            // home archive: index.html + page/N/index.html
            var homePages = PagedContent<Article>.Paginate(newestFirst, PostsPerPage,
                i => _links.Link(i == 1 ? "/" : $"/page/{i}"));
            foreach (var page in homePages)
            {
                var html = await RenderAsync<IndexPage>(new()
                {
                    ["Ctx"] = _ctx,
                    ["Articles"] = page,
                    ["AllArticles"] = newestFirst,
                    ["IsHomeIndex"] = true,
                });
                WriteFile(page.Index == 1 ? "index.html" : $"page/{page.Index}/index.html", html);
            }

            // tag and category archives
            await RenderGroupedArchivesAsync("category", newestFirst,
                a => a.SelectedCategory is { Slug.Length: > 0 } category ? new[] { category.Slug } : Array.Empty<string>(),
                slug => new Category { Slug = slug, Title = slug.SlugToTitle() });
            await RenderGroupedArchivesAsync("tag", newestFirst,
                a => a.TagObjects.Select(t => t.Slug),
                slug => new Tag { Slug = slug, Title = slug.SlugToTitle() });

            async Task RenderGroupedArchivesAsync(
                string pathPrefix, IReadOnlyList<Article> all,
                Func<Article, IEnumerable<string>> getSlugs, Func<string, TaxonomyTerm> createTerm)
            {
                var groups = all
                    .SelectMany(a => getSlugs(a).Select(slug => (slug, article: a)))
                    .GroupBy(x => x.slug);
                foreach (var group in groups)
                {
                    var term = createTerm(group.Key);
                    var groupPages = PagedContent<Article>.Paginate(
                        group.Select(x => x.article).ToList(), PostsPerPage,
                        i => _links.Link(i == 1 ? $"/{pathPrefix}/{group.Key}" : $"/{pathPrefix}/{group.Key}/{i}"));
                    foreach (var page in groupPages)
                    {
                        var html = await RenderAsync<IndexPage>(new()
                        {
                            ["Ctx"] = _ctx,
                            ["Articles"] = page,
                            ["TitleProvider"] = term,
                        });
                        var suffix = page.Index == 1 ? "" : $"{page.Index}/";
                        WriteFile($"{pathPrefix}/{group.Key}/{suffix}index.html", html);
                    }
                }
            }

            // content pages: about-me, 404, …
            foreach (var page in pages)
            {
                var html = await RenderAsync<IndexPage>(new()
                {
                    ["Ctx"] = _ctx,
                    ["Page"] = page,
                    ["TitleProvider"] = page,
                });
                WriteFile(page.Url == "404" ? "404.html" : $"pages/{page.Url}/index.html", html);
            }

            // projects
            WriteFile("pages/projects/index.html",
                await RenderAsync<ProjectsPage>(new() { ["Ctx"] = _ctx, ["Projects"] = projects }));

            // feeds and sitemap
            var feedArticles = newestFirst.Select(a => (a, articleBodies[a.Slug])).ToList();
            FeedWriter.WriteRss(Path.Combine(_outputPath, "feed.rss"), _ctx.Site, _links, feedArticles);
            FeedWriter.WriteAtom(Path.Combine(_outputPath, "feed.atom"), _ctx.Site, _links, feedArticles);
            SitemapWriter.Write(Path.Combine(_outputPath, "sitemap.xml"), _links, articles, homePages.Count);

            var fileCount = Directory.EnumerateFiles(_outputPath, "*", SearchOption.AllDirectories).Count();
            Console.WriteLine($"Generated {fileCount} files in {stopwatch.ElapsedMilliseconds} ms.");
        }

        private List<Article> LoadArticles() =>
            Directory.EnumerateFiles(Path.Combine(_inputPath, "posts"), "*.md")
                .OrderBy(f => f, StringComparer.Ordinal)
                .Select(file =>
                {
                    var doc = MarkdownDocument.Load(file);
                    var fm = doc.FrontMatter;
                    var categorySlug = fm.Category ?? "";
                    return new Article
                    {
                        Title = fm.Title,
                        Description = fm.Description,
                        Slug = doc.Slug,
                        PublishDate = fm.Date,
                        CanonicalUrl = fm.CanonicalUrl,
                        ContentHtml = doc.BodyHtml,
                        SelectedCategory = new Category { Slug = categorySlug, Title = categorySlug.SlugToTitle() },
                        TagObjects = fm.Tags.Select(t => new Tag { Slug = t, Title = t.SlugToTitle() }).ToList(),
                        ArticleAuthor = new Author { Name = "Petr Švihlík" },
                    };
                })
                .ToList();

        private List<Page> LoadPages() =>
            Directory.EnumerateFiles(Path.Combine(_inputPath, "pages"), "*.md")
                .OrderBy(f => f, StringComparer.Ordinal)
                .Select(file =>
                {
                    var doc = MarkdownDocument.Load(file);
                    return new Page
                    {
                        Title = doc.FrontMatter.Title,
                        Url = doc.Slug,
                        Body = doc.BodyHtml,
                    };
                })
                .ToList();

        private List<Project> LoadProjects() =>
            Directory.EnumerateFiles(Path.Combine(_inputPath, "projects"), "_*.md")
                .Select(file =>
                {
                    var doc = MarkdownDocument.Load(file);
                    var fm = doc.FrontMatter;
                    return new Project
                    {
                        Title = fm.Title,
                        Repo = fm.Repo,
                        Logo = fm.Logo,
                        Description = fm.Description,
                        ContentHtml = doc.BodyHtml,
                        Order = fm.Order ?? 999,
                    };
                })
                .OrderBy(p => p.Order)
                .ToList();

        private void CopyStaticAssets()
        {
            // root-level static files (favicons, manifests, …) — everything that isn't content
            foreach (var file in Directory.EnumerateFiles(_inputPath))
            {
                var extension = Path.GetExtension(file).ToLowerInvariant();
                if (extension is ".md" or ".yaml" or ".razor" or ".cshtml")
                {
                    continue;
                }
                CopyFile(file, Path.GetFileName(file));
            }

            // asset folders, verbatim
            var assetsPath = Path.Combine(_inputPath, "assets");
            foreach (var file in Directory.EnumerateFiles(assetsPath, "*", SearchOption.AllDirectories))
            {
                CopyFile(file, Path.GetRelativePath(_inputPath, file));
            }
        }

        private void CopyFile(string sourcePath, string relativeDestination)
        {
            var destination = Path.Combine(_outputPath, relativeDestination);
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            File.Copy(sourcePath, destination, overwrite: true);
        }

        private void WriteFile(string relativePath, string content)
        {
            var destination = Path.Combine(_outputPath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            File.WriteAllText(destination, content);
        }
    }
}
