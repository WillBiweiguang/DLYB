using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class WeldCategoryStatisticsView : IViewModel
	{	
		public Int32 Id { get;set; }

        public int ProjectId { get; set; }

        public int AddressId { get; set; }

        public int HanjieId { get; set; }

        public int WeldLocationId { get; set; }

        public int WeldTypeId { get; set; }

        public int ThicknessId { get; set; }

        public int GrooveTypeId { get; set; }

        public string SectionalArea { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
        
        public IViewModel ConvertAPIModel(object obj) {
            var entity = (WeldCategoryStatistics)obj;
            Id = entity.Id;
            ProjectId = entity.ProjectId;
            AddressId = entity.AddressId;
            HanjieId = entity.HanjieId;
            WeldLocationId = entity.WeldLocationId;
            WeldTypeId = entity.WeldTypeId;
            ThicknessId = entity.ThicknessId;
            GrooveTypeId = entity.GrooveTypeId;
            SectionalArea = entity.SectionalArea;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            return this;
        }
	}
}
