using System.Collections.Generic;
using System.Linq;

namespace PetrSvihlik.Com.Models.ContentTypes
{
    public partial class Author
    {
        public IEnumerable<Contact> ContactsTyped => Contacts.OfType<Contact>();
    }
}