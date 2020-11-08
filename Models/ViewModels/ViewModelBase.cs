﻿using PetrSvihlik.Com.Models.ContentTypes;

namespace PetrSvihlik.Com.Models.ViewModels
{
    public abstract class ViewModelBase
    {
        protected ViewModelBase(SiteMetadata metadata)
        {
            Author = metadata.SiteAuthor;
            Metadata = metadata;
        }

        public Author Author { get; set; }

        public SiteMetadata Metadata { get; set; }
    }
}
