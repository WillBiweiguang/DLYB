using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class WeldingView : IViewModel
	{	
		public Int32 Id { get;set; }

        public string WeldingType { get; set; }

        public string WeldingModel { get; set; }
        
        public string WeldingSpecific { get; set; }
        
        public string WeldingUnit { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
        
        public IViewModel ConvertAPIModel(object obj) {
            var entity = (Welding)obj;
            Id = entity.Id;
            WeldingType = entity.WeldingType;
            WeldingModel = entity.WeldingModel;
            WeldingSpecific = entity.WeldingSpecific;
            WeldingUnit = entity.WeldingUnit;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            return this;
        }
	}
}
