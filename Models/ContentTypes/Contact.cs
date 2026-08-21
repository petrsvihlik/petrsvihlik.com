namespace PetrSvihlik.Com.Models.ContentTypes
{
    public class Contact
    {
        public string Name { get; set; }
        public string Url { get; set; }

        /// <summary>Lightbox mode for the link ("doc" opens Url in the document lightbox instead of navigating).</summary>
        public string Lightbox { get; set; }

        /// <summary>Downloadable file offered by the lightbox toolbar (e.g. the PDF version of a document).</summary>
        public string Download { get; set; }
    }
}
