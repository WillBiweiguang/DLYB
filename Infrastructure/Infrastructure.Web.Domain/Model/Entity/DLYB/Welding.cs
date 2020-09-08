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
    [Table("t_welding")]
    public partial class Welding : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }
        [Column("Welding_Type")]
        public string WeldingType { get; set; }
        [Column("Welding_Model")]
        public string WeldingModel { get; set; }
        [Column("Welding_Specific")]
        public string WeldingSpecific { get; set; }
        [Column("Welding_Unit")]
        public string WeldingUnit { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
    }
}
