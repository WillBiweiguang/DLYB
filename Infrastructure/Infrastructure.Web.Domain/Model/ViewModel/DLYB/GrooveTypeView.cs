using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class GrooveTypeView : IViewModel
	{	
		public Int32 Id { get;set; }

        public string GrooveType { get; set; }
        public string WeldGeometry { get; set; }
        public string Thickness { get; set; }
        public string WorksThicknessH1 { get; set; }
        public string WorksThicknessH2 { get; set; }
        public string GrooveClearance { get; set; }
        public string BluntThickness { get; set; }
        public string GrooveAngleA1 { get; set; }
        public string GrooveAngleA2 { get; set; }
        public string GrooveArcR1 { get; set; }
        public string GrooveArcR2 { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
        
        public IViewModel ConvertAPIModel(object obj) {
            var entity = (GrooveTypes)obj;
            Id = entity.Id;
            GrooveType = entity.GrooveType;
            WeldGeometry = entity.WeldGeometry;
            GrooveType = entity.GrooveType;
            Thickness = entity.Thickness;
            WorksThicknessH1 = entity.WorksThicknessH1;
            WorksThicknessH2 = entity.WorksThicknessH2;
            GrooveClearance = entity.GrooveClearance;
            BluntThickness = entity.BluntThickness;
            GrooveAngleA1 = entity.GrooveAngleA1;
            GrooveAngleA2 = entity.GrooveAngleA2;
            GrooveArcR1 = entity.GrooveArcR1;
            GrooveArcR2 = entity.GrooveArcR2;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            return this;
        }
	}
}
