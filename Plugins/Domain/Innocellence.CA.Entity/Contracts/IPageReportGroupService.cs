using System.Linq.Expressions;
using Infrastructure.Core;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;

namespace DLYB.CA.Contracts
{
    public interface IPageReportGroupService : IDependency, IBaseService<PageReportGroup>
    {
        List<T> GetListByFromDate<T>(Expression<Func<PageReportGroup, bool>> predicate) where T : IViewModel, new();
    }

}
