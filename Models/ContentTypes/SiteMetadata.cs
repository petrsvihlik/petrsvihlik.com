using System.Linq;

namespace PetrSvihlik.Com.Models.ContentTypes
{
    public partial class SiteMetadata
    {
        public Author SiteAuthor => Author.OfType<Author>().First();
    }
}