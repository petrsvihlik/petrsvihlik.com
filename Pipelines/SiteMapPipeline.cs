using PetrSvihlik.Com.Models;
using PetrSvihlik.Com.Models.ContentTypes;
using Statiq.Common;
using Statiq.Core;
using System;
using System.Linq;

namespace PetrSvihlik.Com.Pipelines
{
    public class SiteMapPipeline : Pipeline
    {
        public SiteMapPipeline()
        {
            Dependencies.AddRange(nameof(PostsPipeline), nameof(HomePipeline));
            ProcessModules = new ModuleList(
                // pull documents from other pipelines
                new ReplaceDocuments(Dependencies.ToArray()),
                new SetMetadata(Keys.SitemapItem, Config.FromDocument(doc =>
                    new SitemapItem(doc.Destination.FullPath)
                    {
                        // posts carry their publish date; generated index pages fall back to build time
                        LastModUtc = doc.Get<Article>(MetadataKeys.ArticleModel)?.PublishDate ?? DateTime.UtcNow,
                        ChangeFrequency = SitemapChangeFrequency.Weekly,
                    })),

                new GenerateSitemap()
            );

            OutputModules = new ModuleList {
                new WriteFiles(),
            };
        }
    }
}
