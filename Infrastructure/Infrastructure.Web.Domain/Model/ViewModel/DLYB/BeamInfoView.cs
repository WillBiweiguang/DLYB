using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class BeamInfoView : ViewModelBase, IViewModel
	{	
		public Int32 Id { get;set; }

        public int ProjectId { get; set; }
        public string PdfFile { get; set; }
        public string DwgFile { get; set; }
        public int PageNumber { get; set; }
        public string TheModel { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }
        public string CreatedUserName { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
        public int ProcessStatus { get; set; }
        //�Ǵ洢�ֶ�
        public string ProjectName { get; set; }
        /// <summary>
        /// ��������ͨ��dwgfile�ֶν���
        /// </summary>
        public string BridgeComponent
        {
            get
            {
                if (this.DwgFile.IndexOf("dwg") > -1)
                {
                    return this.DwgFile.Substring(0, this.DwgFile.IndexOf("dwg") - 1);
                }
                return "";
            }
        }

        public int TaskStatus { get; set; }

        public int BeamNum { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (BeamInfo)obj;
            Id = entity.Id;
            ProjectId = entity.ProjectId;
            PdfFile = entity.PdfFile;
            DwgFile = entity.DwgFile;
            TheModel = entity.TheModel;
            Status = entity.Status;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = entity.IsDeleted;
            ProcessStatus = entity.ProcessStatus;
            BeamNum = entity.BeamNum;
            return this;
        }
	}
}
