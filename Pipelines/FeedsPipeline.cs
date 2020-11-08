using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Kontent.Statiq;
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
                // Pull documents from other pipelines
                new ReplaceDocuments(Dependencies.ToArray()),

                // Set metadata for the feeds module
                new SetMetaDataItems(
                    async (input, context) =>
                    {
                        var post = input.AsKontent<Article>();
                        var html = await ParseHtml(input, context);
                        var article = html?.GetElementsByTagName("article").FirstOrDefault()?.InnerHtml ?? "";

                        return new MetadataItems
                        {
                            {FeedKeys.Title, post.Title},
                            {FeedKeys.Content, post.Description},
                            {FeedKeys.Description, post.Description},
                            {FeedKeys.Image, post.OgImage.FirstOrDefault()?.Url},// TODO: make that a local image!
                            {FeedKeys.Published, post.PublishDate},
                            {FeedKeys.Content, article}
                        };
                    }),
                new GenerateFeeds()
                    .WithFeedTitle("Petr Svihlik - blog") //TODO: load from Kontent
                    .WithFeedDescription("Blog about all that matters to me - technology, life, leadership, and Developer Relations.")
                    .WithFeedCopyright($"{DateTime.Today.Year}")
            );
            OutputModules = new ModuleList(
                new WriteFiles()
            );
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