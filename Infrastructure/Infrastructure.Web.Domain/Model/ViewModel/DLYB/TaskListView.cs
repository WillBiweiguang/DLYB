using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Utility.Extensions;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class TaskListView : IViewModel
	{	
		public Int32 Id { get;set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int BeamId { get; set; }
        
        public string DWGFile { get; set; }
        
        public string DWGProcess { get; set; }
        public string ResultProcess { get; set; }

        public int TaskStatus { get; set; }

        public string TaskStatusDes
        {
            get
            {
                return ((Infrastructure.Web.Domain.Common.TaskStatus)this.TaskStatus).ToDescription();
            }
        }

        public string AuditStatus { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
        
        public IViewModel ConvertAPIModel(object obj) {
            var entity = (TaskList)obj;
            Id = entity.Id;
            ProjectId = entity.ProjectId;
            BeamId = entity.BeamId;
            DWGFile = entity.DWGFile;
            DWGProcess = entity.DWGProcess;
            ResultProcess = entity.ResultProcess;
            TaskStatus = entity.TaskStatus;
            AuditStatus = entity.AuditStatus;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            return this;
        }
	}
}
