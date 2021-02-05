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
    [Table("t_loginlog")]
    public partial class LoginLog : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }

        public string UserTrueName { get; set; }

        public string UserName { get; set; }

        public string IpAddress { get; set; }

        public string Operation { get; set; }

        public DateTime OperationDate { get; set; }

        public string AffiliatedInstitution { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserID { get; set; }

        public bool IsDeleted { get; set; }
    }
}
