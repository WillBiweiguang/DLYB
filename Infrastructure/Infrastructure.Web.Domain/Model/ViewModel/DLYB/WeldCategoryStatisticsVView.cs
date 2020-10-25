using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class WeldCategoryStatisticsVView : ViewModelBase, IViewModel
	{	
		public Int32 Id { get;set; }
                
        public string ProjectName { get; set; }
        
        public string AddressName { get; set; }
        //
        public string HanJieType { get; set; }
        
        public string WeldLocationType { get; set; }
        //∫∏∑Ï¿‡–Õ
        public string WeldType { get; set; }
        
        public string ThickType { get; set; }
        
        public string GrooveType { get; set; }
        
        public string SectionalArea { get; set; }

        public string WeldingModel { get; set; }
        
        //public string WeldingType { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

        public int BeamId { get; set; }

        public IViewModel ConvertAPIModel(object obj) {
            var entity = (WeldCategoryStatisticsV)obj;
            Id = entity.Id;
            ProjectName = entity.ProjectName;
            AddressName = entity.AddressName;
            HanJieType = entity.HanJieType;
            WeldLocationType = entity.WeldLocationType;
            WeldType = entity.WeldType;
            ThickType = entity.ThickType;
            GrooveType = entity.GrooveType;
            SectionalArea = entity.SectionalArea;
            WeldingModel =entity.WeldingModel;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            BeamId = entity.BeamId;
            return this;
        }
	}
}
