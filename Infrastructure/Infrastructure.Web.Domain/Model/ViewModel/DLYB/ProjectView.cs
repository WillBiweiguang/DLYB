using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class ProjectView : IViewModel
	{	
		public Int32 Id { get;set; }
        public string ProjectName { get; set; }

        public string ProjectType { get; set; }

        public string AffiliatedInstitution { get; set; }

        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }
        public string CreatedUserName { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public string UpdateUserName { get; set; }

        public bool IsDeleted { get; set; }

        public string DepartmentID { get; set; }

        public string DepartmentName { get; set; }

        public IViewModel ConvertAPIModel(object obj) {
            var entity = (Project)obj;
            Id = entity.Id;
            ProjectName = entity.ProjectName;
            ProjectType = entity.ProjectType;
            AffiliatedInstitution = entity.AffiliatedInstitution;
            Status = entity.Status;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            DepartmentID = entity.DepartmentID;
            return this;
        }
	}
}
