using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class WeldCognitionView : IViewModel
	{	
		public Int32 Id { get;set; }

        public string FileName { get; set; }

        public int FileID { get; set; }

        public string WeldType { get; set; }

        public int HandleID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
        
        public IViewModel ConvertAPIModel(object obj) {
            var entity = (WeldCognition)obj;
            Id = entity.Id;
            FileID = entity.FileID;
            FileName = entity.FileName;
            HandleID = entity.HandleID;
            WeldType = entity.WeldType;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            return this;
        }
	}
}
