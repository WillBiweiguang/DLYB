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
    [Table("t_ProjectInfo")]
    public partial class Project : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }
       
        public string ProjectName { get; set; }

        public int? ProjectType { get; set; }

        public string AffiliatedInstitution { get; set; }

        public string Status { get; set; }
        public string create_by { get; set; }

        public DateTime create_time { get; set; }

        public DateTime? update_time { get; set; }

        public bool IsDeleted { get; set; }

    }
}
