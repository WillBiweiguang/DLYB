using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using DLYB.CA.Entity;
using System.ComponentModel;

namespace DLYB.CA.ModelsView
{
    //[Table("CourseAttention")]
    public partial class PageReportView : IViewModel
    {

        public Int32 Id { get; set; }

        [DescriptionAttribute("����")]
        public string GroupName { get; set; }
        public string AppName { get; set; }
        public string PageUrl { get; set; }

        [DescriptionAttribute("��������")]
        public string AccessDate { get; set; }
        //��������
        [DescriptionAttribute("��������")]
        public int? VisitorCount { get; set; }
        //���ʴ���
        [DescriptionAttribute("���ʴ���")]
        public int? VisitTimes { get; set; }

        //����������
         [DescriptionAttribute("��������")]
        public int? TotalVisitorCount { get; set; }
        //�����ܴ���
        [DescriptionAttribute("���ʴ���")]
        public int? TotalVisitTimes { get; set; }
        //	public  TrainingCourseView TrainingCourse { get;set; }


        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (PageReport)obj;
            Id = entity.Id;
            PageUrl = entity.PageUrl;
            AccessDate = entity.AccessDate == null ? "" : ((DateTime)entity.AccessDate).ToString("yyyy-MM-dd");
            VisitorCount = entity.VisitorCount;
            VisitTimes = entity.VisitTimes;
            AppName = entity.AppName;
            return this;
        }

        public IViewModel ConvertAPIModel(object obj, string groupName, string appName)
        {
            var entity = (PageReport)obj;
            Id = entity.Id;
            PageUrl = entity.PageUrl;
            AccessDate = entity.AccessDate == null ? "" : ((DateTime)entity.AccessDate).ToString("yyyy-MM-dd");
            VisitorCount = entity.VisitorCount;
            VisitTimes = entity.VisitTimes;
            GroupName = groupName;
            AppName = appName;

            return this;
        }
    }
}
