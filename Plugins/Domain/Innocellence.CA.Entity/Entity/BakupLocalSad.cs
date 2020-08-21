using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Contracts.Entity
{
    public partial class BakupLocalSad : EntityBase<int>
    {
        public override Int32 Id { get; set; }

        public string ChineseName { get; set; }

        public string LillyID { get; set; }

        public string Email { get; set; }

        public string Gender { get; set; }

        public string Company { get; set; }

        public string Department { get; set; }

        public string SubDepartment { get; set; }

        public int? DeptId { get; set; }

        public string Mobile { get; set; }

        public string Tags { get; set; }

        public int OrderNum { get; set; }

        public DateTime? BakupDT { get; set; }
    }
}
