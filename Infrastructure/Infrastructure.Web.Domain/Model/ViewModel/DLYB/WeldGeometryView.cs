using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class WeldGeometryView : ViewModelBase, IViewModel
	{	
		public Int32 Id { get;set; }

        public string WeldType { get; set; }

        public string WeldImage { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedUserName { get; set; }

        public IViewModel ConvertAPIModel(object obj) {
            var entity = (WeldGeometry)obj;
            Id = entity.Id;
            WeldType = entity.WeldType;
            WeldImage = entity.WeldImage;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            return this;
        }
	}
}
