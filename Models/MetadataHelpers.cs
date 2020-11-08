namespace PetrSvihlik.Com.Models
{
    public static class MetadataHelpers
    {
        public static T Cascade<T>(this T preferred, T fallback) => preferred switch
        {
            string s => string.IsNullOrWhiteSpace(s) ? fallback : preferred,
            _ => preferred ?? fallback,
        };
    }
}