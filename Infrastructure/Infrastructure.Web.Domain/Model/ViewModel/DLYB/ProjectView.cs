using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class ProjectView : IViewModel
	{	
		public int Id { get;set; }
        public string ProjectName { get; set; }

        public int? ProjectType { get; set; }

        public string AffiliatedInstitution { get; set; }

        public string Status { get; set; }
        public string create_by { get; set; }

        public DateTime create_time { get; set; }

        public DateTime? update_time { get; set; }

        public bool IsDeleted { get; set; }

        public IViewModel ConvertAPIModel(object obj) {
            var entity = (Project)obj;
            Id = entity.Id;
            ProjectName = entity.ProjectName;
            ProjectType = entity.ProjectType;
            AffiliatedInstitution = entity.AffiliatedInstitution;
            Status = entity.Status;
            create_by = entity.create_by;
            create_time = entity.create_time;
            update_time = entity.update_time;
            IsDeleted = entity.IsDeleted;
            return this;
        }
	}
}
