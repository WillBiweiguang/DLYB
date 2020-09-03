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
    [Table("t_beaminfo")]
    public partial class BeamInfo : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }
        [Column("Project_Id")]
        public int ProjectId { get; set; }
        [Column("BeamSection_Name")]
        public string BeamSectionName { get; set; }
        [Column("Figure_Number")]
        public string FigureNumber { get; set; }
        [Column("Beam_Length")]
        public string BeamLength { get; set; }
        [Column("Beam_Width")]
        public string BeamWidth { get; set; }
        [Column("Beam_Thick")]
        public string BeamThick { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
    }
}
