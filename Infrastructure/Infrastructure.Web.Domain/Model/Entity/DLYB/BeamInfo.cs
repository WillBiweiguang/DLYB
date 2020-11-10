using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Web.Domain.Entity
{
    [Table("t_BeamInfo")]
    public partial class BeamInfo : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }
        public int ProjectId { get; set; }
        public string PdfFile { get; set; }
        public string DwgFile { get; set; }
        public int PageNumber { get; set; }
        public string TheModel { get; set; }
        public string Status { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

        public int ProcessStatus { get; set; }

        public int BeamNum { get; set; }
    }
}
