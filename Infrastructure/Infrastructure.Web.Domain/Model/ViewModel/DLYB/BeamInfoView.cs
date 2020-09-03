using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class BeamInfoView : IViewModel
	{	
		public Int32 Id { get;set; }

        public int ProjectId { get; set; }
        
        public string BeamSectionName { get; set; }
        
        public string FigureNumber { get; set; }
        
        public string BeamLength { get; set; }
        
        public string BeamWidth { get; set; }
        
        public string BeamThick { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (BeamInfo)obj;
            Id = entity.Id;
            ProjectId = entity.ProjectId;
            BeamSectionName = entity.BeamSectionName;
            FigureNumber = entity.FigureNumber;
            BeamLength = entity.BeamLength;
            BeamWidth = entity.BeamWidth;
            BeamThick = entity.BeamThick;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            return this;
        }
	}
}
