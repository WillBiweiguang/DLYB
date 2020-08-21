using System;
using Infrastructure.Core;

namespace DLYB.CA.Contracts.Entity
{
    public partial class BatchJobLog : EntityBase<int>
    {
        public override int Id { get; set; }

        public string JobID { get; set; }

        public int? Status { get; set; }

        public string Type { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Result { get; set; }
    }
}
