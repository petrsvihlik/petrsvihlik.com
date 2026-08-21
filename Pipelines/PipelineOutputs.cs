using PetrSvihlik.Com.Models;
using PetrSvihlik.Com.Models.ContentTypes;
using PetrSvihlik.Com.Models.ViewModels;
using Statiq.Common;
using System.Collections.Generic;
using System.Linq;

namespace PetrSvihlik.Com.Pipelines
{
    /// <summary>
    /// Typed accessors for the singleton models and collections other pipelines publish,
    /// replacing the repeated FromPipeline/Select/FirstOrDefault chains in every pipeline.
    /// </summary>
    public static class PipelineOutputs
    {
        public static SiteMetadata GetSiteMetadata(this IExecutionContext context) =>
            context.Outputs.FromPipeline(nameof(SiteMetadataPipeline))
                .Select(x => x.Get<SiteMetadata>(MetadataKeys.SiteMetadata))
                .FirstOrDefault();

        public static Homepage GetHomepage(this IExecutionContext context) =>
            context.Outputs.FromPipeline(nameof(HomepagePipeline))
                .Select(x => x.Get<Homepage>(MetadataKeys.Homepage))
                .FirstOrDefault();

        /// <summary>All posts, newest first.</summary>
        public static IReadOnlyList<Article> GetArticles(this IExecutionContext context) =>
            context.Outputs.FromPipeline(nameof(PostsPipeline))
                .Select(d => d.Get<Article>(MetadataKeys.ArticleModel))
                .Where(a => a != null)
                .OrderByDescending(a => a.PublishDate)
                .ToList();

        public static SidebarViewModel CreateSidebar(this IExecutionContext context, bool isIndex = false, string activeMenuItem = null) =>
            new(context.GetHomepage(), context.GetSiteMetadata(), isIndex, activeMenuItem);
    }
}
