using System.Linq.Expressions;
using Infrastructure.Core;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;

namespace DLYB.CA.Contracts
{
    public interface IAppAccessReportService : IDependency, IBaseService<AppAccessReport>
    {
        List<T> GetListByFromDate<T>(Expression<Func<AppAccessReport, bool>> predicate) where T : IViewModel, new();
    }

}
