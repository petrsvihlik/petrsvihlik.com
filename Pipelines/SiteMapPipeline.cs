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
                new SetMetadata(Keys.SitemapItem, Config.FromDocument((doc, ctx) =>
                {
                    var siteMapItem = new SitemapItem(doc.Destination.FullPath)
                    {
                        LastModUtc = DateTime.UtcNow,
                        ChangeFrequency = SitemapChangeFrequency.Weekly,
                    };
                    return siteMapItem;
                })),

                new GenerateSitemap()
            );

            OutputModules = new ModuleList {
                new WriteFiles(),
            };
        }
    }
}
