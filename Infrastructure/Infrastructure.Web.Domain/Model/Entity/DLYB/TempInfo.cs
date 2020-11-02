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
    [Table("t_TempInfo")]
    public partial class TempInfo : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string AffiliatedInstitution { get; set; }
        public long AffiliatedId { get; set; }
        public string BridgeType { get; set; }
        public long BridgeTypeId { get; set; }
        public long FileId { get; set; }
        public string FileStatus { get; set; }
        public long RelationId { get; set; }
        public string RelationFigureId { get; set; }
        public string FigureNumber { get; set; }
        public string BoardNumber { get; set; }
        public double Thickness { get; set; }
        //焊缝数量
        public double WeldNum { get; set; }
        //梁段数量
        public double BeamNum { get; set; }
        public double LengthVal { get; set; }
        public double WidthVal { get; set; }
        //public DateTime? CreatedDate { get; set; }

        //public int? CreatedUserID { get; set; }

        //public DateTime? UpdatedDate { get; set; }

        //public int? UpdatedUserID { get; set; }

        //public bool IsDeleted { get; set; }

    }
}
