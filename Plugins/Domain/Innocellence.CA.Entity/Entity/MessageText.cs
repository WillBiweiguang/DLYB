using System;
using Infrastructure.Core;


namespace DLYB.CA.Entity
{
	//[Table("MessageText")]
    public partial class MessageText : EntityBase<int>
	{
        public Int32? AppId { get; set; }
        public Int32? RefId { get; set; }
        public String EventName { get; set; }
        public String Content { get; set; }
        public String Status { get; set; }
        public bool IsDeleted { get; set; }
        public String toDepartment { get; set; }
        public String toTag { get; set; }
        public String toUser { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String CreatedUserID { get; set; }
        public String UpdatedUserID { get; set; }

        public string MessageType { get; set; }

        public string RefEntity { get; set; }

	}

}
