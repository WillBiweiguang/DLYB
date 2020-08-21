using System;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Core;

namespace DLYB.CA.Contracts.Entity
{
    public class PushHistoryDetailEntity : EntityBase<int>
    {
        public int HistoryId { get; set; }

        public string ErrorUsers { get; set; }

        public string ErrorTags { get; set; }

        public string ErrorDepartments { get; set; }

        public string ErrorType { get; set; }

        public string CreatedUserId { get; set; }

        public string UpdatedUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("HistoryId")]
        public virtual PushHistoryEntity History { get; set; }
    }
}
