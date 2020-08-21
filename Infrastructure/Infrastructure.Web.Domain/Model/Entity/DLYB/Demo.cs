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
    [Table("demo")]
    public partial class Demo : EntityBase<int>
    {
        public override int Id { get => base.Id; set => base.Id = value; }
        public string Name
        {
            get;
            set;
        }
        [Column("create_time")]
        public DateTime CreateTime
        {
            get;
            set;
        }
        [Column("update_time")]
        public DateTime UpdateTime { get; set; }
        [Column("state")]
        public int State { get; set; }
    }
}
