using System;
using System.Collections.Generic;
using Infrastructure.Core;
using Infrastructure.Utility.Data;
using DLYB.CA.Entity;
using System.ComponentModel;


namespace DLYB.CA.ModelsView
{
    //[Table("MessageText")]
    public partial class MessageTextView : IViewModel
    {

        public Int32 Id { get; set; }
        public Int32? AppId { get; set; }
        public Int32? RefId { get; set; }
        public String EventName { get; set; }

        [DescriptionAttribute("文字内容")]
        public String Content { get; set; }
        public String Status { get; set; }
        public String toDepartment { get; set; }
        public String toTag { get; set; }
        public String toUser { get; set; }

         [DescriptionAttribute("发布日期")]
        public DateTime? PublishDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String CreatedUserID { get; set; }
        public String UpdatedUserID { get; set; }
        public bool IsDeleted { get; set; }
        public String AppName { get; set; }
        public int HistoryId { get; set; }
        public string EventPersonCategory { get; set; }

        public string MessageType { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (MessageText)obj;
            Id = entity.Id;
            AppId = entity.AppId;
            RefId = entity.RefId;
            EventName = entity.EventName;
            Content = entity.Content;
            Status = entity.Status;
            PublishDate = entity.PublishDate;
            toDepartment = entity.toDepartment;
            toTag = entity.toTag;
            toUser = entity.toUser;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            IsDeleted = entity.IsDeleted;
            EventPersonCategory = string.IsNullOrEmpty(entity.RefEntity) ? null : string.Join(",", JsonHelper.FromJson<IList<string>>(entity.RefEntity));

            return this;
        }
    }
}
