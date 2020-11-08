namespace PetrSvihlik.Com.Extensions
{
    public static class StringExtensions
    {
        public static string TwitterHandle(this string handle) => handle.StartsWith('@') ? handle : $"@{handle}";
    }
}
