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
    [Table("t_weldcategorystatistics")]
    public partial class WeldCategoryStatistics : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }
        [Column("Project_Id")]
        public int ProjectId { get; set; }
        [Column("Address_Id")]
        public int AddressId { get; set; }
        [Column("Hanjie_Id")]
        public int HanjieId { get; set; }
        [Column("WeldLocation_Id")]
        public int WeldLocationId { get; set; }
        [Column("WeldType_Id")]
        public int WeldTypeId { get; set; }
        [Column("Thickness_Id")]
        public int ThicknessId { get; set; }
        [Column("GrooveType_Id")]
        public int GrooveTypeId { get; set; }
        [Column("Sectional_Area")]
        public string SectionalArea { get; set; }
        [Column("Welding_Id")]
        public int WeldingId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

        public int BeamId { get; set; }
    }
}
