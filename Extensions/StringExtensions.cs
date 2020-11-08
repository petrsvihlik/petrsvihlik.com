namespace Kentico.Kontent.Statiq.Lumen.Extensions
{
    public static class StringExtensions
    {
        public static string TwitterHandle(this string handle) => handle.StartsWith('@') ? handle : $"@{handle}";
    }
}
