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
    [Table("t_v_weldcategorystatistics")]
    public partial class WeldCategoryStatisticsV : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }
        [Column("ProjectName")]
        public string ProjectName { get; set; }
        [Column("Address_Name")]
        public string AddressName { get; set; }
        [Column("HanJieType")]
        public string HanJieType { get; set; }
        [Column("WeldLocationType")]
        public string WeldLocationType { get; set; }
        [Column("WeldType")]
        public string WeldType { get; set; }
        [Column("ThickType")]
        public string ThickType { get; set; }
        [Column("GrooveType")]
        public string GrooveType { get; set; }
        [Column("Sectional_Area")]
        public string SectionalArea { get; set; }
        [Column("Welding_Model")]
        public string WeldingModel { get; set; }
        //[Column("Welding_Type")]
        //public string WeldingType { get; set; }
        public string DepartmentID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

        public int BeamId { get; set; }
    }
}
