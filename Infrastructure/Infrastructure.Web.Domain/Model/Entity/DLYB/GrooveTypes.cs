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
    [Table("t_groovetypes")]
    public partial class GrooveTypes : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }
        public string GrooveType { get; set; }
        public string WeldGeometry { get; set; }
        public double Thickness { get; set; }
        public double? WorksThicknessH1 { get; set; }
        public double? WorksThicknessH2 { get; set; }
        public double? GrooveClearance { get; set; }
        public double? BluntThickness { get; set; }
        public double? GrooveAngleA1 { get; set; }
        public double? GrooveAngleA2 { get; set; }
        public double? GrooveArcR1 { get; set; }
        public double? GrooveArcR2 { get; set; }
        public double? CircleArcR { get; set; }
        public double? GrooveThicknessT { get; set; }
        public string PreviewImage { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
    }
}
