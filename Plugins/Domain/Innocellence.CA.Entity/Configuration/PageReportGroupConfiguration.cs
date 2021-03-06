﻿using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class PageReportGroupConfiguration : EntityConfigurationBase<PageReportGroup, int>
    {
        public PageReportGroupConfiguration()
        {
            ToTable("PageReportGroup");
        }
    }
}
