using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class WeldCategoryStatisticsViewModel: IViewModel
    {	
		public Int32 Id { get;set; }

        public int ProjectId { get; set; }

        public int AddressId { get; set; }

        public int HanjieId { get; set; }

        public int WeldLocationId { get; set; }

        public int WeldTypeId { get; set; }

        public int ThicknessId { get; set; }

        public int GrooveTypeId { get; set; }

        public int WeldingId { get; set; }

        public string SectionalArea { get; set; }

        public string ProjectName { get; set; }

        public string AddressName { get; set; }

        public string HanJieType { get; set; }

        public string WeldLocationType { get; set; }

        public string WeldType { get; set; }

        public string ThickType { get; set; }

        public string GrooveType { get; set; }

        public string WeldingModel { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
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
            WeldingModel = entity.WeldingModel;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            return this;
        }

        public WeldCategoryStatisticsView ConvertView()
        {
            var view = new WeldCategoryStatisticsView
            {
                Id = this.Id,
                ProjectId = this.ProjectId,
                AddressId = this.AddressId,
                HanjieId = this.HanjieId,
                WeldLocationId = this.WeldLocationId,
                WeldTypeId = this.WeldTypeId,
                ThicknessId = this.ThicknessId,
                GrooveTypeId = this.GrooveTypeId,
                SectionalArea = this.SectionalArea,
                WeldingId = this.WeldingId,
                CreatedDate = this.CreatedDate,
                CreatedUserID = this.CreatedUserID,
                UpdatedDate = this.UpdatedDate,
                UpdatedUserID = this.UpdatedUserID,
                IsDeleted = this.IsDeleted
            };
            return view;
        }
        
	}
}
