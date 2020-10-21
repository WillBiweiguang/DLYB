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
    [Table("t_BoardPackage")]
    public partial class BoardPackage : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }
        public int BeamId { get; set; }
        public string FigureNumber { get; set; }
        public string BoardNumber { get; set; }
        public double Thickness { get; set; }
        public double WeldLength { get; set; }
        public double LengthVal { get; set; }
        public double WidthVal { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }

    }
}
