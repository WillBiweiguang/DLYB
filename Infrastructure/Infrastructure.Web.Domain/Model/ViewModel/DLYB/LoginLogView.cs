using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class LoginLogView : ViewModelBase, IViewModel
	{	
		public Int32 Id { get;set; }

        public string UserTrueName { get; set; }

        public string UserName { get; set; }

        public string IpAddress { get; set; }

        public string Operation { get; set; }

        public DateTime OperationDate { get; set; }

        public string AffiliatedInstitution { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }
        public string CreatedUserName { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (LoginLog)obj;
            Id = entity.Id;
            UserTrueName = entity.UserTrueName;
            UserName = entity.UserName;
            IpAddress = entity.IpAddress;
            Operation = entity.Operation;
            OperationDate = entity.OperationDate;
            AffiliatedInstitution = entity.AffiliatedInstitution;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            return this;
        }
	}
}
