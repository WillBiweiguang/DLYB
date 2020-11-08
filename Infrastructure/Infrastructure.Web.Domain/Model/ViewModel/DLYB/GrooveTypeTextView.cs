using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class GrooveTypeTextView : ViewModelBase, IViewModel
    {
        public Int32 Id { get; set; }
        public string GrooveType { get; set; }
        public string WeldGeometry { get; set; }
        public string Thickness { get; set; }
        public string WorksThickness1 { get; set; }
        public string WorksThickness2 { get; set; }
        public string WorksThicknessH1 { get; set; }
        public string WorksThicknessH2 { get; set; }
        public string WorksThicknessH3 { get; set; }
        public string WeldLeg1 { get; set; }
        public string GrooveClearance { get; set; }
        public string BluntThickness { get; set; }
        public string GrooveAngleA1 { get; set; }
        public string GrooveAngleA2 { get; set; }
        public string GrooveArcR1 { get; set; }
        public string GrooveArcR2 { get; set; }
        public string CircleArcR { get; set; }
        public string GrooveThicknessT { get; set; }
        public string PreviewImage { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }
        public string CreatedUserName { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (GrooveTypesText)obj;
            Id = entity.Id;
            GrooveType = entity.GrooveType;
            WeldGeometry = entity.WeldGeometry;
            GrooveType = entity.GrooveType;
            Thickness = entity.Thickness;
            WorksThickness1 = entity.WorksThickness1;
            WorksThickness2 = entity.WorksThickness2;
            WorksThicknessH1 = entity.WorksThicknessH1;
            WorksThicknessH2 = entity.WorksThicknessH2;
            WorksThicknessH3 = entity.WorksThicknessH3;
            WeldLeg1 = entity.WeldLeg1;
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
