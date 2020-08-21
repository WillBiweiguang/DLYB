using System;
using Infrastructure.Core;
using Infrastructure.Utility.IO;
using DLYB.CA.Contracts.Entity;
using System.ComponentModel;

namespace DLYB.CA.Contracts.ViewModel
{
    public class ArticleReportView : IViewModel
    {
        [CsvIgnore]
        public Int32 Id { get; set; }

        [CsvIgnore]
        public Int32 AppId { get; set; }

        public Int32 ArticleId { get; set; }

        [CsvIgnore]
        [DescriptionAttribute("菜单key")]
        public string MenuKey { get; set; }

        [CsvIgnore]
        [DescriptionAttribute("菜单名称")]
        public string MenuName { get; set; }

        public string AppName { get; set; }

        [DescriptionAttribute("文章标题")]
        public string ArticleTitle { get; set; }

        [CsvIgnore]
        public string CreatedDate { get; set; }

        [DescriptionAttribute("访问日期")]
        public string AccessDate { get; set; }

        [DescriptionAttribute("访问人数")]
        public int VisitorCount { get; set; }

        [DescriptionAttribute("访问次数")]
        public int VisitTimes { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (ArticleReport)obj;
            Id = entity.Id;
            AppId = entity.AppId;
            ArticleId = entity.ArticleId;
            MenuKey = entity.MenuKey;
            MenuName = entity.MenuName;
            AppName = entity.AppName;
            ArticleTitle = entity.ArticleTitle;
            CreatedDate = entity.CreatedDate == null ? "" : ((DateTime)entity.CreatedDate).ToString("yyyy-MM-dd");
            AccessDate = entity.AccessDate == null ? "" : ((DateTime)entity.AccessDate).ToString("yyyy-MM-dd");
            VisitorCount = entity.VisitorCount;
            VisitTimes = entity.VisitTimes;

            return this;
        }
    }
}
