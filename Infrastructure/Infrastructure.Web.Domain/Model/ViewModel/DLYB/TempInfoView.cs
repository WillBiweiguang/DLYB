using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class TempInfoView : ViewModelBase, IViewModel
	{
        public Int32 Id { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string BeamName { get; set; }
        public string FileName { get; set; }
        public string AffiliatedInstitution { get; set; }
        public string AffiliatedId { get; set; }
        public string BridgeType { get; set; }
        public string BridgeTypeId { get; set; }
        public string FileId { get; set; }
        public string FileStatus { get; set; }
        public string RelationId { get; set; }
        public string RelationFigureId { get; set; }
        public string FigureNumber { get; set; }
        public string BoardNumber { get; set; }
        public string BarNumber { get; set; }
        public double? Thickness { get; set; }
        //焊缝数量
        public double? WeldNum { get; set; }
        //梁段数量
        public double? BeamNum { get; set; }
        public double? LengthVal { get; set; }
        public double? WidthVal { get; set; }
       // public DateTime? CreatedDate { get; set; }

        //public int? CreatedUserID { get; set; }

        //public DateTime? UpdatedDate { get; set; }

        //public int? UpdatedUserID { get; set; }

        //public bool IsDeleted { get; set; }
        
        public IViewModel ConvertAPIModel(object obj) {
            var entity = (TempInfo)obj;
            Id = entity.Id;
            ProjectId = entity.ProjectId;
            ProjectName = entity.ProjectName;
            FileName = entity.FileName;
            BeamName = entity.BeamName;
            AffiliatedInstitution = entity.AffiliatedInstitution;
            AffiliatedId = entity.AffiliatedId;
            BridgeType = entity.BridgeType;
            BridgeTypeId = entity.BridgeTypeId;
            FileId = entity.FileId;
            FileStatus = entity.FileStatus;
            RelationId = entity.RelationId;
            RelationFigureId = entity.RelationFigureId;
            FigureNumber = entity.FigureNumber;
            BoardNumber = entity.BoardNumber;
            Thickness = entity.Thickness;
            LengthVal = entity.LengthVal;
            WidthVal = entity.WidthVal;
            WeldNum = entity.WeldNum;
            BeamNum = entity.BeamNum;
            //CreatedDate = entity.CreatedDate;
            //CreatedUserID = entity.CreatedUserID;
            //UpdatedDate = entity.UpdatedDate;
            //UpdatedUserID = entity.UpdatedUserID;
            //IsDeleted = entity.IsDeleted;
            return this;
        }
	}
}
