using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;
using PetrSvihlik.Com.Models.ContentTypes;
using PetrSvihlik.Com.Modules;
using Statiq.Common;
using Statiq.Core;
using Statiq.Feeds;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PetrSvihlik.Com.Pipelines
{
    public class FeedsPipeline : Pipeline
    {
        public FeedsPipeline()
        {
            Dependencies.Add(nameof(PostsPipeline));
            ProcessModules = new ModuleList(
                new ReplaceDocuments(Dependencies.ToArray()),
                new SetMetaDataItems(
                    async (input, context) =>
                    {
                        var article = input.Get<Article>("ArticleModel");
                        var html = await ParseHtml(input, context);
                        var articleContent = html?.GetElementsByTagName("article").FirstOrDefault()?.InnerHtml ?? "";

                        return new MetadataItems
                        {
                            { FeedKeys.Title, article?.Title },
                            { FeedKeys.Description, article?.Description },
                            { FeedKeys.Published, article?.PublishDate },
                            { FeedKeys.Content, articleContent },
                        };
                    }),
                new GenerateFeeds()
                    .WithFeedTitle("Petr Svihlik - blog")
                    .WithFeedDescription("Blog about all that matters to me - technology, life, leadership, and Developer Relations.")
                    .WithFeedCopyright($"{DateTime.Today.Year}")
            );
            OutputModules = new ModuleList(new WriteFiles());
        }

        private static async Task<IHtmlDocument> ParseHtml(IDocument document, IExecutionContext context)
        {
            var parser = new HtmlParser();
            try
            {
                await using var stream = document.GetContentStream();
                return await parser.ParseDocumentAsync(stream);
            }
            catch (Exception ex)
            {
                context.LogWarning("Exception while parsing HTML for {0}: {1}", document.ToSafeDisplayString(), ex.Message);
            }
            return null;
        }
    }
}
