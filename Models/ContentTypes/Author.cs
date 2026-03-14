using System.Collections.Generic;

namespace PetrSvihlik.Com.Models.ContentTypes
{
    public class Author
    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public List<Contact> Contacts { get; set; } = new();
    }
}
