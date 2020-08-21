using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Contracts.Entity
{
    public class LocalSadPassword : EntityBase<int>
    {
        public override Int32 Id { get; set; }

        public string Pwd { set; get; }

        public DateTime? UpdateTime { set; get; }
    }
}
