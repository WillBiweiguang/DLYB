using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;

namespace Infrastructure.Web.Domain.Entity
{
    public partial class ReleaseNote : EntityBase<int>
    {

        public override Int32 Id { get; set; }

        public DateTime ReleaseDate { get; set; }

        public String Name { get; set; }
        public String Version { get; set; }
        public String Note { get; set; }
      
    }
}
