using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;

using DLYB.CA.Entity;
using System.ComponentModel;

namespace DLYB.CA.ModelsView
{
    public partial class AppAccessReportView : IViewModel
    {
        public int Id { get; set; }
        [DescriptionAttribute("访问日期")]
        public string AccessDate { get; set; }

        public int? AppId { get; set; }

        public string CreatedDate { get; set; }

        public string AppName { get; set; }

        [DescriptionAttribute("访问人数")]
        public int? AccessPerson { get; set; }
        [DescriptionAttribute("访问次数")]
        public int? AccessCount { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (AppAccessReport)obj;
            Id = entity.Id;
            AccessDate = entity.AccessDate == null ? "" : ((DateTime)entity.AccessDate).ToString("yyyy-MM-dd");
            AppId = entity.AppId;
            CreatedDate = entity.CreatedDate == null ? "" : ((DateTime)entity.CreatedDate).ToString("yyyy-MM-dd");
            AppName = entity.AppName;
            AccessPerson = entity.AccessPerson;
            AccessCount = entity.AccessCount;
            return this;
        }
    }
}
