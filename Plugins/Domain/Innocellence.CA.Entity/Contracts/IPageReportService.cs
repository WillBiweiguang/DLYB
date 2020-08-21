using System.Linq.Expressions;
using Infrastructure.Core;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;
using DLYB.CA.ModelsView;
using Infrastructure.Utility.Data;

namespace DLYB.CA.Contracts
{
    public interface IPageReportService : IDependency, IBaseService<PageReport>
    {
        List<T> GetListByFromDate<T>(Expression<Func<PageReport, bool>> predicate) where T : IViewModel, new();
        List<PageReportView> GetReportList(Expression<Func<PageReport, bool>> predicate,
              PageCondition con);
    }

}
