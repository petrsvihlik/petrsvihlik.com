using PetrSvihlik.Com.Models.ContentTypes;
using PetrSvihlik.Com.Models.ViewModels;
using Statiq.Common;
using Statiq.Core;
using Statiq.Markdown;
using Statiq.Razor;
using Statiq.Yaml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetrSvihlik.Com.Pipelines
{
    public class PostsPipeline : Pipeline
    {
        public PostsPipeline()
        {
            Dependencies.AddRange(nameof(SiteMetadataPipeline));
            InputModules = new ModuleList
            {
                new ReadFiles("posts/*.md"),
                new ExtractFrontMatter(new ParseYaml()),
                new RenderMarkdown(),
                new SetMetadata("ArticleModel", Config.FromDocument((doc, ctx) => BuildArticle(doc))),
                new SetMetadata(nameof(Category), Config.FromDocument(doc =>
                    doc.GetString("category"))),
                new SetMetadata(nameof(Tag), Config.FromDocument(doc =>
                    doc.GetList<string>("tags", new List<string>()))),
                new SetDestination(Config.FromDocument(doc =>
                    new NormalizedPath($"posts/{doc.GetString("slug") ?? doc.Source.FileNameWithoutExtension}.html"))),
            };

            ProcessModules = new ModuleList
            {
                new MergeContent(new ReadFiles("_Post.cshtml")),
                new RenderRazor()
                    .WithModel(Config.FromDocument((document, context) =>
                    {
                        var article = document.Get<Article>("ArticleModel");
                        var metadata = context.Outputs.FromPipeline(nameof(SiteMetadataPipeline))
                            .Select(x => x.Get<SiteMetadata>("SiteMetadata")).FirstOrDefault();
                        return new PostViewModel(article, metadata);
                    })),
            };

            OutputModules = new ModuleList
            {
                new WriteFiles(),
            };
        }

        private static Article BuildArticle(IDocument doc)
        {
            var categorySlug = doc.GetString("category") ?? "";
            var tagSlugs = doc.GetList<string>("tags", new List<string>());

            return new Article
            {
                Title = doc.GetString("title"),
                Description = doc.GetString("description"),
                Slug = doc.GetString("slug") ?? doc.Source.FileNameWithoutExtension.FullPath,
                PublishDate = doc.Get<DateTime?>("date"),
                CanonicalUrl = doc.GetString("canonical_url"),
                ContentHtml = doc.GetContentStringAsync().GetAwaiter().GetResult(),
                SelectedCategory = new Category
                {
                    Slug = categorySlug,
                    Title = SlugToTitle(categorySlug)
                },
                TagObjects = tagSlugs.Select(s => new Tag { Slug = s, Title = SlugToTitle(s) }).ToList(),
                ArticleAuthor = new Author { Name = "Petr Švihlík" }
            };
        }

        internal static string SlugToTitle(string slug)
        {
            if (string.IsNullOrEmpty(slug)) return slug;
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo
                .ToTitleCase(slug.Replace("-", " "));
        }
    }
}
