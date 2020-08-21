using System;
using System.Collections.Generic;
using Infrastructure.Core;

namespace DLYB.CA.Entity
{
	//[Table("Message")]
    public partial class Message : EntityBase<int>
	{
        public override Int32 Id { get; set; }
        public Int32? AppId { get; set; }
        //public Int32? EventId { get; set; }
        public String EventName { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
        public String URL { get; set; }

        public Guid? Code { get; set; }
        public String Comment { get; set; }
        public String Status { get; set; }
        public Int32? ReadCount { get; set; }
        public Int32? ThumbsUpCount { get; set; }
        public Boolean? IsLike { get; set; }
        public bool IsDeleted { get; set; }
        public Int32? ThumbImageId { get; set; }
        public String ThumbImageUrl { get; set; }
        public DateTime? PublishDate { get; set; }
        public String Previewers { get; set; }
        public String toDepartment { get; set; }
        public String toTag { get; set; }
        public String toUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String CreatedUserID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String UpdatedUserID { get; set; }

        public string MessageType { get; set; }

        public string RefEntity { get; set; }

        public int? RefId { get; set; }
	}


    public class CheckResult
    {
        public IList<Desc> ErrorDepartments { get; set; }

        public IList<Desc> ErrorTags { get; set; }

        public IList<string> ErrorUsers { get; set; }
    }

    public class Desc
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public enum MessageType
    {
        Messsage,
        EventMessage
    }

    public enum EventPersonCategory
    {
        Checkin,
        Registered
    }
}
