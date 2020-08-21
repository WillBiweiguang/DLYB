using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Contracts.Configuration
{
    public class SubmitInfoConfiguration : EntityConfigurationBase<SubmitInfo, int>
    {
        public SubmitInfoConfiguration()
        {
            ToTable("SubmitInfo");
        }
    }
}
