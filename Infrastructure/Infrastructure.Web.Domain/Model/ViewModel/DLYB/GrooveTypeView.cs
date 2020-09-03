using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class GrooveTypeView : IViewModel
	{	
		public Int32 Id { get;set; }

        public string AddressName { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
        
        public IViewModel ConvertAPIModel(object obj) {
            var entity = (GrooveType)obj;
            Id = entity.Id;
            AddressName = entity.AddressName;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            return this;
        }
	}
}
