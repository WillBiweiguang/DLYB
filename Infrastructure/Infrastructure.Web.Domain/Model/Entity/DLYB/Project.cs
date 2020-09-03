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
    [Table("project_info")]
    public partial class Project : EntityBase<string>
    {
        [Column("Address_Name")]
        public string Name { get; set; }

        public int? type { get; set; }

        public string img_url { get; set; }

        public int? audit_state { get; set; }

        public int? bridge_type_id { get; set; }

        public string design_institute_id { get; set; }

        public string descript { get; set; }

        public string create_by { get; set; }

        public string dept_id { get; set; }

        public string dept_name { get; set; }

        public int? progress_state { get; set; }

        public DateTime create_time { get; set; }

        public DateTime? update_time { get; set; }

        public int? state { get; set; }
    }
}
