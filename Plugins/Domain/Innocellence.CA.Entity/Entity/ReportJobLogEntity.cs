using System;
using Infrastructure.Core;

namespace DLYB.CA.Contracts.Entity
{
    public class ReportJobLogEntity : EntityBase<int>
    {
        public override int Id { get; set; }

        public string JobName { get; set; }

        public string JobStatus { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
