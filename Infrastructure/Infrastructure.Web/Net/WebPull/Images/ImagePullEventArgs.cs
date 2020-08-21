// -----------------------------------------------------------------------
//  <copyright file="ImagePullEventArgs.cs" company="DLYB">
//      Copyright (c) 2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 12:29</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Infrastructure.Web.Net.WebPull.Images
{
    public class GroupDownloadEventArgs : EventArgs
    {
        public Group Group { get; set; }

        public int Count { get; set; }
    }


    public class ImageDownloadEventArgs : EventArgs
    {
        public string Url { get; set; }

        public string FileName { get; set; }

        public int Count { get; set; }
    }


    public class ImageUrlGetEventArgs : EventArgs
    {
        public string ForumName { get; set; }

        public string GroupName { get; set; }

        public string ImageUrl { get; set; }
    }


    public class GroupGetEventArgs : EventArgs
    {
        public Forum Forum { get; set; }
    }


    public class WebClientErrorEventArgs : EventArgs
    {
        public string Url { get; set; }

        public string Message { get; set; }
    }
}