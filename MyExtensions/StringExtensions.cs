using System.Globalization;

namespace PetrSvihlik.Com.Extensions
{
    public static class StringExtensions
    {
        public static string TwitterHandle(this string handle) => handle.StartsWith('@') ? handle : $"@{handle}";

        /// <summary>
        /// Turns a URL slug ("developer-relations") into a display title ("Developer Relations").
        /// </summary>
        public static string SlugToTitle(this string slug) =>
            string.IsNullOrEmpty(slug)
                ? slug
                : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(slug.Replace('-', ' '));
    }
}
