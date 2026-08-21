using PetrSvihlik.Com.Models.ContentTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;

namespace PetrSvihlik.Com.Generation
{
    /// <summary>Writes sitemap.xml: all posts (with publish date as lastmod) plus the home archive pages.</summary>
    public static class SitemapWriter
    {
        public static void Write(string path, SiteLinks links, IReadOnlyList<Article> articles, int homePageCount)
        {
            var now = DateTime.UtcNow;
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            foreach (var article in articles)
            {
                AppendUrl(xml, links.Absolute($"/posts/{article.Slug}"), article.PublishDate ?? now);
            }

            for (var i = 1; i <= homePageCount; i++)
            {
                AppendUrl(xml, links.Absolute(i == 1 ? "/" : $"/page/{i}"), now);
            }

            xml.Append("</urlset>");
            File.WriteAllText(path, xml.ToString());
        }

        private static void AppendUrl(StringBuilder xml, string location, DateTime lastModified)
        {
            xml.Append("<url><loc>").Append(SecurityElement.Escape(location)).Append("</loc>")
               .Append("<lastmod>").Append(lastModified.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)).Append("</lastmod>")
               .Append("<changefreq>weekly</changefreq></url>");
        }
    }
}
