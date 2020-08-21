using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using DLYB.CA.Entity;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Contracts.ViewModel;
using System.ComponentModel;

namespace DLYB.CA.ModelsView
{
    public partial class FeedBackView : IViewModel
    {
        public Int32 Id { get; set; }
        [DescriptionAttribute("内容")]
        public String Content { get; set; }
        public String MenuCode { get; set; }

        [DescriptionAttribute("提问者")]
        public String FeedBackUserId { get; set; }

        [DescriptionAttribute("提问日期")]
        public DateTime FeedBackDateTime { get; set; }
        public Int32 AppID { get; set; }
        public FBConfig FB { get; set; }
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (FeedBackEntity)obj;
            Id = entity.Id;
            Content = entity.Content;
            MenuCode = entity.MenuCode;
            FeedBackUserId = entity.FeedBackUserId;
            FeedBackDateTime = entity.FeedBackDateTime;
            AppID = entity.AppID;
            return this;
        }
    }
}
