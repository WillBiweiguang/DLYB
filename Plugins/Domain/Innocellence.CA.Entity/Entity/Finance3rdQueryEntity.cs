using System;
using Infrastructure.Core;

namespace DLYB.CA.Contracts.Entity
{
    public class Finance3rdQueryEntity : EntityBase<int>
    {
        public override int Id { get; set; }
        public string VenderName { get; set; }
        public string ContractNO { get; set; }
        public string MeetingCode { get; set; }
        public DateTime MeetingTime { get; set; }
        public string MeetingPlace { get; set; }
        public string LillyId { get; set; }
        public Decimal MoneySum { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }
    }
}
