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
    [Table("t_TaskList")]
    public partial class TaskList : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }

        public int ProjectId { get; set; }

        public int BeamId { get; set; }

        public string DWGFile { get; set; }

        public string DWGProcess { get; set; }
        public string ResultProcess { get; set; }

        public int TaskStatus { get; set; }

        public string AuditStatus { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
    }
}
