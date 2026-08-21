using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;

namespace PetrSvihlik.Com.Generation
{
    /// <summary>A sitemap URL: its rooted location and, when meaningful, a last-modified date.</summary>
    public sealed record SitemapEntry(string RootedUrl, DateTime? LastModified);

    /// <summary>Writes sitemap.xml from the entries the build collects.</summary>
    public static class SitemapWriter
    {
        public static void Write(string path, SiteLinks links, IReadOnlyList<SitemapEntry> entries)
        {
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            foreach (var entry in entries)
            {
                xml.Append("<url><loc>").Append(SecurityElement.Escape(links.ToAbsolute(entry.RootedUrl))).Append("</loc>");
                if (entry.LastModified is { } lastModified)
                {
                    xml.Append("<lastmod>").Append(lastModified.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)).Append("</lastmod>");
                }
                xml.Append("</url>");
            }

            xml.Append("</urlset>");
            File.WriteAllText(path, xml.ToString());
        }
    }
}
