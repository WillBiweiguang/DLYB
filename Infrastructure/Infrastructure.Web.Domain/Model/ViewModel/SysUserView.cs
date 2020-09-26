using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
	//[Table("Push")]
    public partial class SysUserView : IViewModel
	{
	
		public Int32 Id { get;set; }

        public String UserId { get; set; }

        public String UserName { get; set; }
        public String Email { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String CreatedUserID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String UpdatedUserID { get; set; }

        public String UserTrueName { get; set; }

        public string SecurityStamp { get; set; }

        public String PhoneNumber { get; set; }

        public String PasswordHash { get; set; }

        public String strRoles { get; set; }


        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (SysUser)obj;
            Id = entity.Id;
            UserId = entity.UserId;
            UserName = entity.UserName;
            Email = entity.Email;
            PhoneNumber = entity.PhoneNumber;
            UserTrueName = entity.UserTrueName;
            PasswordHash = entity.PasswordHash;
            SecurityStamp = entity.SecurityStamp;

            return this;
        }
	}
}
