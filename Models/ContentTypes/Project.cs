namespace PetrSvihlik.Com.Models.ContentTypes
{
    public class Project
    {
        public string Title { get; set; }
        public string Repo { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }
        public string ContentHtml { get; set; }
        public int Order { get; set; }

        public string GitHubUrl => string.IsNullOrEmpty(Repo) ? null : $"https://github.com/{Repo}";
    }
}
