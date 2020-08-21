using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using DLYB.CA.Entity;
using System.ComponentModel;

namespace DLYB.CA.ModelsView
{
    public partial class WechatFollowReportView : IViewModel
    {
        public Int32 Id { get; set; }

        [DescriptionAttribute("统计日期")]
        public String StatisticsDate { get; set; }

        [DescriptionAttribute("跟踪数")]
        public Int32? FollowCount { get; set; }

        [DescriptionAttribute("未跟踪数")]
        public Int32? UnFollowCount { get; set; }

        public string CreatedDate { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (WechatFollowReport)obj;
            Id = entity.Id;
            StatisticsDate = entity.StatisticsDate.HasValue ?
                Convert.ToDateTime(entity.StatisticsDate).ToString("yyyy-MM-dd") : string.Empty;
            FollowCount = entity.FollowCount;
            UnFollowCount = entity.UnFollowCount;
            CreatedDate = entity.CreatedDate == null ? "" : ((DateTime)entity.CreatedDate).ToString("yyyy-MM-dd");

            return this;
        }
    }
}
