using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using DLYB.CA.Entity;

namespace DLYB.CA.ModelsView 
{
    public partial class FileManageView : IViewModel
	{
        public Int32 Id { get; set; }

        public String FileName { get; set; }

        public String FileType { get; set; }

        public String FileSize { get; set; }

        public String Url { get; set; }

        public String FilePath { get; set; }

        public String OriginalName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public String CreatedUserID { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (FileManage)obj;
            Id = entity.Id;
            FileName = entity.FileName;
            FileType = entity.FileType;
            FileSize = entity.FileSize;
            Url = entity.Url;
            FilePath = entity.FilePath;
            OriginalName = entity.OriginalName;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;

            return this;
        }
	}
}
