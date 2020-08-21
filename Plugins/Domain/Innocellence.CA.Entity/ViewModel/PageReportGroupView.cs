using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using DLYB.CA.Entity;
namespace DLYB.CA.Entity
{
    public partial class PageReportGroupView : IViewModel
    {
        public int Id { get; set; }
        //分组名称
        public string GroupName { get; set; }
        //分组编码
        public string GroupCode { get; set; }
        public string PageUrl { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public Int32? AppId { get; set; }
        public String AppName { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (PageReportGroup)obj;
            Id = entity.Id;
            GroupName = entity.GroupName;
            GroupCode = entity.GroupCode;
            PageUrl = entity.PageUrl;
            CreatedDate = entity.CreatedDate == null ? "" : ((DateTime)entity.CreatedDate).ToString("yyyy-MM-dd");
            UpdatedDate = entity.UpdatedDate == null ? "" : ((DateTime)entity.UpdatedDate).ToString("yyyy-MM-dd");
            AppId = entity.AppId;

            return this;
        }
    }
}
