using System;
using System.Collections.Generic;
using System.Linq;
using Kentico.Kontent.Delivery.Abstractions;

namespace Kentico.Kontent.Statiq.Lumen.Models
{
    public class CustomTypeProvider : ITypeProvider
    {
        private static readonly Dictionary<Type, string> _codenames = new Dictionary<Type, string>
        {
            {typeof(Article), "article"},
            {typeof(Author), "author"},
            {typeof(Category), "category"},
            {typeof(Contact), "contact"},
            {typeof(Homepage), "homepage"},
            {typeof(Page), "page"},
            {typeof(SiteMetadata), "site_metadata"},
            {typeof(Tag), "tag"}
        };

        public Type GetType(string contentType)
        {
            return _codenames.Keys.FirstOrDefault(type => GetCodename(type).Equals(contentType));
        }

        public string GetCodename(Type contentType)
        {
            return _codenames.TryGetValue(contentType, out var codename) ? codename : null;
        }
    }
}