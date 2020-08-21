﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;
using System.Linq.Expressions;

namespace DLYB.CA.Contracts
{    
    public interface IWechatFollowReportService : IDependency, IBaseService<WechatFollowReport>
    {
        List<T> GetListByFromStatisticsDate<T>(Expression<Func<WechatFollowReport, bool>> predicate) where T : IViewModel, new();
    }
}
