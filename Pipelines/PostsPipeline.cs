using PetrSvihlik.Com.Extensions;
using PetrSvihlik.Com.Models;
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
using System.Threading.Tasks;

namespace PetrSvihlik.Com.Pipelines
{
    public class PostsPipeline : Pipeline
    {
        public PostsPipeline()
        {
            Dependencies.AddRange(nameof(SiteMetadataPipeline), nameof(HomepagePipeline));
            InputModules = new ModuleList
            {
                new ReadFiles("posts/*.md"),
                new ExtractFrontMatter(new ParseYaml()),
                new RenderMarkdown().UseExtensions(),
                new SetMetadata(MetadataKeys.ArticleModel, Config.FromDocument(async doc => (object)await BuildArticleAsync(doc))),
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
                        new PostViewModel(
                            document.Get<Article>(MetadataKeys.ArticleModel),
                            context.GetSiteMetadata(),
                            context.CreateSidebar()))),
            };

            OutputModules = new ModuleList
            {
                new WriteFiles(),
            };
        }

        private static async Task<Article> BuildArticleAsync(IDocument doc)
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
                ContentHtml = await doc.GetContentStringAsync(),
                SelectedCategory = new Category
                {
                    Slug = categorySlug,
                    Title = categorySlug.SlugToTitle()
                },
                TagObjects = tagSlugs.Select(s => new Tag { Slug = s, Title = s.SlugToTitle() }).ToList(),
                ArticleAuthor = new Author { Name = "Petr Švihlík" }
            };
        }
    }
}
