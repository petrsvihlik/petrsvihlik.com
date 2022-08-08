using Kontent.Ai.Delivery.Abstractions;
using PetrSvihlik.Com.Models.ContentTypes;
using System.Threading.Tasks;

namespace PetrSvihlik.Com.Resolvers
{
    public class CustomContentLinkUrlResolver : IContentLinkUrlResolver
    {
        public Task<string> ResolveLinkUrlAsync(IContentLink link)
        {
            // Resolves URLs to content items based on the 'accessory' content type
            return Task.FromResult(link.ContentTypeCodename switch
            {
                Article.Codename => $"/post/{link.UrlSlug}",
                Page.Codename => $"/pages/{link.UrlSlug}",
                Category.Codename => $"/category/{link.UrlSlug}",
                Tag.Codename => $"/tag/{link.UrlSlug}",
                _ => "/404",
            });
        }

        public Task<string> ResolveBrokenLinkUrlAsync()
        {
            // Resolves URLs to unavailable content items
            return Task.FromResult("/404");
        }
    }
}