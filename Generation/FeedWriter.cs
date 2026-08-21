using PetrSvihlik.Com.Models.ContentTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PetrSvihlik.Com.Generation
{
    /// <summary>Writes the RSS 2.0 and Atom feeds (all posts, newest first, full article HTML as content).</summary>
    public static class FeedWriter
    {
        private const string FeedTitle = "Petr Svihlik - blog";
        private const string FeedDescription = "Blog about all that matters to me - technology, life, leadership, and Developer Relations.";

        private static readonly XNamespace ContentNs = "http://purl.org/rss/1.0/modules/content/";
        private static readonly XNamespace AtomNs = "http://www.w3.org/2005/Atom";

        public static void WriteRss(string path, SiteMetadata site, SiteLinks links, IReadOnlyList<(Article Article, string Html)> articles)
        {
            var now = DateTime.UtcNow;
            var channel = new XElement("channel",
                new XElement("title", FeedTitle),
                new XElement("link", links.Absolute("/")),
                new XElement("description", FeedDescription),
                new XElement("copyright", now.Year.ToString(CultureInfo.InvariantCulture)),
                new XElement("pubDate", Rfc1123(now)),
                new XElement("lastBuildDate", Rfc1123(now)));

            foreach (var (article, html) in articles)
            {
                var link = links.Absolute($"/posts/{article.Slug}");
                channel.Add(new XElement("item",
                    new XElement("title", article.Title),
                    new XElement("link", link),
                    new XElement("description", article.Description),
                    new XElement("guid", new XAttribute("isPermaLink", "false"), link),
                    new XElement("pubDate", Rfc1123(article.PublishDate ?? now)),
                    new XElement(ContentNs + "encoded", html)));
            }

            var rss = new XElement("rss",
                new XAttribute(XNamespace.Xmlns + "content", ContentNs),
                new XAttribute("version", "2.0"),
                channel);

            Save(path, rss);
        }

        public static void WriteAtom(string path, SiteMetadata site, SiteLinks links, IReadOnlyList<(Article Article, string Html)> articles)
        {
            var now = DateTime.UtcNow;
            var feed = new XElement(AtomNs + "feed",
                new XElement(AtomNs + "id", links.Absolute("/")),
                new XElement(AtomNs + "title", FeedTitle),
                new XElement(AtomNs + "link", new XAttribute("rel", "self"), new XAttribute("href", links.Absolute("/"))),
                new XElement(AtomNs + "rights", now.Year.ToString(CultureInfo.InvariantCulture)),
                new XElement(AtomNs + "updated", Iso8601(now)),
                new XElement(AtomNs + "subtitle", FeedDescription));

            foreach (var (article, html) in articles)
            {
                var link = links.Absolute($"/posts/{article.Slug}");
                feed.Add(new XElement(AtomNs + "entry",
                    new XElement(AtomNs + "id", link),
                    new XElement(AtomNs + "title", article.Title),
                    new XElement(AtomNs + "link", new XAttribute("href", link)),
                    new XElement(AtomNs + "updated", Iso8601(article.PublishDate ?? now)),
                    new XElement(AtomNs + "content", html),
                    new XElement(AtomNs + "summary", article.Description)));
            }

            Save(path, feed);
        }

        private static string Rfc1123(DateTime utc) => utc.ToString("R", CultureInfo.InvariantCulture);

        private static string Iso8601(DateTime utc) => utc.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        private static void Save(string path, XElement root)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true),
            };
            using var writer = XmlWriter.Create(path, settings);
            new XDocument(root).Save(writer);
        }
    }
}
