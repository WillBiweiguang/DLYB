using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Entity
{
    public class UserInfo : EntityBase<int>
    {
        public override Int32 Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LillyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Tel { get; set; }
    }
}
