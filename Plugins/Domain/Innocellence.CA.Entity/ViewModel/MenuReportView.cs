using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using System.ComponentModel;

namespace DLYB.CA.Contracts.ViewModel
{
    public class MenuReportView : IViewModel
    {
        public Int32 Id { get; set; }
        public string UserId { get; set; }

        [DescriptionAttribute("菜单key")]
        public string Menukey { get; set; }

        [DescriptionAttribute("访问日期")]
        public string AccessDate { get; set; }

        public int AppID { get; set; }

        [DescriptionAttribute("菜单名称")]
        public string MenuName { get; set; }

        public string AppName { get; set; }

        [DescriptionAttribute("访问人数")]
        public int VisitorCount { get; set; }

        [DescriptionAttribute("访问次数")]
        public int VisitorTimes { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (DLYB.CA.Contracts.Entity.MenuReportEntity)model;
            Id = entity.Id;
            UserId = entity.UserId;
            Menukey = entity.MenuKey;
            AccessDate = entity.AccessDate == null ? "" : ((DateTime)entity.AccessDate).ToString("yyyy-MM-dd");
            AppID = entity.AppId;
            MenuName = entity.MenuName;
            AppName = entity.AppName;
            VisitorCount = entity.VisitorCount;
            VisitorTimes = entity.VisitTimes;
            return this;
        }
    }
}
