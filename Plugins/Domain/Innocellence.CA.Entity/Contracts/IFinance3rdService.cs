using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrastructure.Core;
using Infrastructure.Utility.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.ModelsView;


namespace DLYB.CA.Contracts.Contracts
{
    public interface IFinance3rdService : IBaseService<Finance3rdQueryEntity>, IDependency
    {
        ResultEntity AddOrUpdate(IList<Finance3rdQueryEntity> list);
        List<T> GetList<T>(Expression<Func<Finance3rdQueryEntity, bool>> predicate) where T : IViewModel, new();
        List<Finance3rdQueryEntityView> GetFinanceList(string lillyid, PageCondition condition, out int totalRow, int month=9999);
    }

   
}
