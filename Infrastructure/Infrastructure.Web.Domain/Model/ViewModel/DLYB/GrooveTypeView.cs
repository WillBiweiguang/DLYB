using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class GrooveTypeView : IViewModel
    {
        public Int32 Id { get; set; }

        public string GrooveType { get; set; }
        public string WeldGeometry { get; set; }
        public double Thickness { get; set; }
        public double? WorksThickness1 { get; set; }
        public double? WorksThickness2 { get; set; }
        public double? WorksThicknessH1 { get; set; }
        public double? WorksThicknessH2 { get; set; }
        public double? GrooveClearance { get; set; }
        public double? BluntThickness { get; set; }
        public double? GrooveAngleA1 { get; set; }
        public double? GrooveAngleA2 { get; set; }
        public double? GrooveArcR1 { get; set; }
        public double? GrooveArcR2 { get; set; }
        public double? CircleArcR { get; set; }
        public double? GrooveThicknessT { get; set; }
        public string PreviewImage { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (GrooveTypes)obj;
            Id = entity.Id;
            GrooveType = entity.GrooveType;
            WeldGeometry = entity.WeldGeometry;
            GrooveType = entity.GrooveType;
            Thickness = entity.Thickness;
            WorksThickness1 = entity.WorksThickness1;
            WorksThickness2 = entity.WorksThickness2;
            WorksThicknessH1 = entity.WorksThicknessH1;
            WorksThicknessH2 = entity.WorksThicknessH2;
            GrooveClearance = entity.GrooveClearance;
            BluntThickness = entity.BluntThickness;
            GrooveAngleA1 = entity.GrooveAngleA1;
            GrooveAngleA2 = entity.GrooveAngleA2;
            GrooveArcR1 = entity.GrooveArcR1;
            GrooveArcR2 = entity.GrooveArcR2;
            CircleArcR = entity.CircleArcR;
            GrooveThicknessT = entity.GrooveThicknessT;
            PreviewImage = entity.PreviewImage;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            return this;
        }
    }
}
