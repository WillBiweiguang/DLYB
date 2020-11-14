using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class WeldCategoryLabelingView : ViewModelBase, IViewModel
	{	
		public Int32 Id { get;set; }

        public int BeamId { get; set; }

        [Required(ErrorMessage = "图号是必填项")]
        public string FigureNumber { get; set; }

        [Required(ErrorMessage = "板件号是必填项")]
        public string BoardNumber { get; set; }

        [Required(ErrorMessage = "焊缝类型是必填项")]
        public string WeldType { get; set; }

        [Required(ErrorMessage ="板厚是必填项")]
        public double Thickness { get; set; }

        [Required(ErrorMessage = "焊缝位置是必填项")]
        public string WeldLocation { get; set; }

        [Required(ErrorMessage = "消耗系数是必填项")]
        public double ConsumeFactor { get; set; }

        //[Required(ErrorMessage = "熔敷密度是必填项")]
        public double MentalDensity { get; set; }

        [Required(ErrorMessage = "焊缝截面积是必填项")]
        public double SectionArea { get; set; }

        //[Required(ErrorMessage = "焊缝长度是必填项")]
        public double WeldLength { get; set; }

        [Required(ErrorMessage = "焊缝质量是必填项")]
        public double WeldQuanlity { get; set; }
        
        public string WeldingNumber { get; set; }

        [Required(ErrorMessage = "焊缝数量是必填项")]
        public double Quantity { get; set; }
        public double? BeamNum { get; set; }
        public double? LengthVal { get; set; }
        public double? WidthVal { get; set; }
        public double? WeldingQuanlity { get; set; }

        public string WeldingType { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

        public string HandleID { get; set; }

        public int? CopyOriginId { get; set; }

        public string CircleId { get; set; }

        public string Ids { get; set; }

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
            WeldingType = entity.WeldingType;
            HandleID = entity.HandleID;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            CircleId = entity.CircleId;
            return this;
        }
	}
}
