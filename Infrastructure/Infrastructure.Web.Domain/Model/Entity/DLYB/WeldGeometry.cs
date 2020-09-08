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
    [Table("t_weldgeometry")]
    public partial class WeldGeometry : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }
        [Column("Weld_Type")]
        public string WeldType { get; set; }
        [Column("Weld_image")]
        public string WeldImage { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
    }
}
