using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class BoardPackageView : ViewModelBase, IViewModel
	{	
		public Int32 Id { get;set; }

        public int BeamId { get; set; }
        public string FigureNumber { get; set; }
        public string BoardNumber { get; set; }
        public double Thickness { get; set; }
        public double LengthVal { get; set; }
        public double WidthVal { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

        public string HandleID { get; set; }

        public int? CopyOriginId { get; set; }

        public IViewModel ConvertAPIModel(object obj) {
            var entity = (BoardPackage)obj;
            Id = entity.Id;
            BeamId = entity.BeamId;
            FigureNumber = entity.FigureNumber;
            BoardNumber = entity.BoardNumber;
            Thickness = entity.Thickness;
            Thickness = entity.Thickness;
            LengthVal = entity.LengthVal;
            WidthVal = entity.WidthVal;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            return this;
        }
	}
}
