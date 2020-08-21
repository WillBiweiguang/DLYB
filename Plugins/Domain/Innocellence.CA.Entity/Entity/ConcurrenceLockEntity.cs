using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Core;

namespace DLYB.CA.Contracts.Entity
{
    [Table("ConcurrenceLock")]
    public class ConcurrenceLockEntity : EntityBase<int>
    {
        public override int Id { get; set; }

        public string Code { get; set; }

        public bool IsLocked { get; set; }
    }
}
