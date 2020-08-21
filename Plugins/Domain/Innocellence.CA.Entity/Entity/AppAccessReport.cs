using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using DLYB.CA.Entity;
namespace DLYB.CA.Entity
{
    public partial class AppAccessReport : EntityBase<int>
    {
        public override int Id { get; set; }
        //访问时间
        public DateTime? AccessDate { get; set; }
        //创建时间
        public DateTime? CreatedDate { get; set; }
        public int? AppId { get; set; }
        public string AppName { get; set; }
        public int? AccessPerson { get; set; }
        public int? AccessCount { get; set; }
    }
}
