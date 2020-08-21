using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Contracts.Entity
{
    public partial class ResultLocalSad : EntityBase<int>
    {
        public override Int32 Id { get; set; }

        public string ChineseName { get; set; }

        public string LillyID { get; set; }

        public string Email { get; set; }

        public string Gender { get; set; }

        public string Company { get; set; }

        public string Department { get; set; }

        public string SubDepartment { get; set; }

        public int OrderNum { get; set; }

        public DateTime? BakupDT { get; set; }

        public int ExecResult { get; set; }

        public string Description { get; set; }

        public int Flag { get; set; }
    }
}
