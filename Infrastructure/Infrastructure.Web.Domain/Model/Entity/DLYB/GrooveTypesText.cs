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
    [Table("t_groovetypesText")]
    public partial class GrooveTypesText : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }
        public string GrooveType { get; set; }
        public string WeldGeometry { get; set; }
        public string Thickness { get; set; }
        public string WorksThickness1 { get; set; }
        public string WorksThickness2 { get; set; }
        public string WorksThicknessH1 { get; set; }
        public string WorksThicknessH2 { get; set; }
        public string WorksThicknessH3 { get; set; }
        public string WeldLeg1 { get; set; }
        public string GrooveClearance { get; set; }
        public string BluntThickness { get; set; }
        public string GrooveAngleA1 { get; set; }
        public string GrooveAngleA2 { get; set; }
        public string GrooveArcR1 { get; set; }
        public string GrooveArcR2 { get; set; }
        public string CircleArcR { get; set; }
        public string GrooveThicknessT { get; set; }
        public string PreviewImage { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
    }
}
