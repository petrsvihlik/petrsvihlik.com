namespace PetrSvihlik.Com.Models
{
    /// <summary>
    /// Names of the custom metadata entries the pipelines pass between each other,
    /// so the keys live in one place instead of being scattered string literals.
    /// </summary>
    public static class MetadataKeys
    {
        public const string ArticleModel = "ArticleModel";
        public const string SiteMetadata = "SiteMetadata";
        public const string Homepage = "Homepage";
        public const string ProjectModel = "ProjectModel";
        public const string ProjectsViewModel = "ProjectsViewModel";
        public const string RenderedBody = "RenderedBody";
        public const string SelectedGroup = "SelectedGroup";
    }
}
