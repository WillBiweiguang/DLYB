using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrastructure.Core;
using Infrastructure.Utility.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.ModelsView;


namespace DLYB.CA.Contracts.Contracts
{
    public interface IFinanceService : IBaseService<FinanceQueryEntity>, IDependency
    {
        ResultEntity AddOrUpdate(IList<FinanceQueryEntity> list);
        List<T> GetList<T>(Expression<Func<FinanceQueryEntity, bool>> predicate) where T : IViewModel, new();
        List<FinanceEntityView> GetFinanceList(string lillyid, PageCondition condition, out int totalRow, int month=9999);
        //List<FinanceEntityView> GetFinanceThirdList(string lillyid, PageCondition condition, out int totalRow);
    }

    public class ResultEntity
    {
        public int InsertCount { get; set; }

        public int UpdateCount { get; set; }
    }
   
}
