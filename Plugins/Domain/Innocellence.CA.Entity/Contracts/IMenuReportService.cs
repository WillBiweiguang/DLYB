using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrastructure.Core;
using Infrastructure.Utility.Data;
using DLYB.CA.Contracts.ViewModel;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Contracts
{
    public interface IMenuReportService : IDependency, IBaseService<MenuReportEntity>
    {
        List<MenuReportView> QueryTable(DateTime stardate, DateTime enddate, PageCondition pageCondition);

        IList<MenuReportEntity> QueryList(Expression<Func<MenuReportEntity, bool>> func);

        List<T> GetListByDate<T>(Expression<Func<MenuReportEntity, bool>> predicate) where T : IViewModel, new();
    }
}
