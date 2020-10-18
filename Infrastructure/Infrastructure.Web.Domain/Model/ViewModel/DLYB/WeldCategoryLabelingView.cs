using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class WeldCategoryLabelingView : ViewModelBase, IViewModel
	{	
		public Int32 Id { get;set; }

        public int BeamId { get; set; }
        public string FigureNumber { get; set; }
        public string BoardNumber { get; set; }
        public string WeldType { get; set; }
        public double Thickness { get; set; }
        public string WeldLocation { get; set; }
        public double ConsumeFactor { get; set; }
        public double MentalDensity { get; set; }
        public double SectionArea { get; set; }
        public double WeldLength { get; set; }
        public double WeldQuanlity { get; set; }
        public string WeldingNumber { get; set; }
        public double Quantity { get; set; }
        public double BeamNum { get; set; }
        public double LengthVal { get; set; }
        public double WidthVal { get; set; }
        public double WeldingQuanlity { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

        public string HandleID { get; set; }

        public int? CopyOriginId { get; set; }

        public IViewModel ConvertAPIModel(object obj) {
            var entity = (WeldCategoryLabeling)obj;
            Id = entity.Id;
            BeamId = entity.BeamId;
            FigureNumber = entity.FigureNumber;
            BoardNumber = entity.BoardNumber;
            Thickness = entity.Thickness;
            WeldType = entity.WeldType;
            Thickness = entity.Thickness;
            WeldLocation = entity.WeldLocation;
            ConsumeFactor = entity.ConsumeFactor;
            MentalDensity = entity.MentalDensity;
            SectionArea = entity.SectionArea;
            WeldLength = entity.WeldLength;
            WeldQuanlity = entity.WeldQuanlity;
            WeldingNumber = entity.WeldingNumber;
            WeldingQuanlity = entity.WeldQuanlity * entity.ConsumeFactor;
            BeamNum = entity.BeamNum;
            Quantity = entity.Quantity;
            LengthVal = entity.LengthVal;
            WidthVal = entity.WidthVal;
            HandleID = entity.HandleID;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            return this;
        }
	}
}
