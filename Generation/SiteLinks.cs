namespace PetrSvihlik.Com.Generation
{
    /// <summary>
    /// Builds site links: root-relative by default (honoring the LinkRoot setting for
    /// subfolder hosting), absolute when a Host is configured (used by feeds and the sitemap).
    /// </summary>
    public sealed class SiteLinks
    {
        private readonly string _root;
        private readonly string _host;

        public SiteLinks(string linkRoot, string host)
        {
            _root = string.IsNullOrWhiteSpace(linkRoot) ? "" : "/" + linkRoot.Trim('/');
            _host = string.IsNullOrWhiteSpace(host) ? null : host.Trim().Trim('/');
        }

        /// <summary>True when a Host is configured and absolute URLs can be built.</summary>
        public bool HasHost => _host is not null;

        /// <summary>Root-relative link, e.g. "/posts/slug".</summary>
        public string Link(string path) => _root + EnsureRooted(path);

        /// <summary>Absolute link when Host is configured, root-relative otherwise.</summary>
        public string Absolute(string path) => _host is null ? Link(path) : $"https://{_host}{Link(path)}";

        /// <summary>Prefixes an already-rooted URL (a Link() result) with the host, when configured.</summary>
        public string ToAbsolute(string rootedUrl) => _host is null ? rootedUrl : $"https://{_host}{rootedUrl}";

        private static string EnsureRooted(string path) =>
            string.IsNullOrEmpty(path) ? "/" : (path.StartsWith('/') ? path : "/" + path);
    }
}
