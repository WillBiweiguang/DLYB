using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.ModelsView;

namespace DLYB.CA.Service
{
    public class Finance3rdService : BaseService<Finance3rdQueryEntity>, IFinance3rdService
    {
        public Finance3rdService()
            : base("CAAdmin")
        {

        }

        public ResultEntity AddOrUpdate(IList<Finance3rdQueryEntity> list)
        {
            Repository.Insert(list.AsEnumerable());

            return new ResultEntity { InsertCount = list.Count };
        }

        public List<T> GetList<T>(Expression<Func<Finance3rdQueryEntity, bool>> predicate) where T : IViewModel, new()
        {
            var lst =Repository.Entities.Where(predicate).ToList().Select(n => (T) (new T().ConvertAPIModel(n))).ToList();
            return lst;
        }

        public List<Finance3rdQueryEntityView> GetFinanceList(string lillyid, PageCondition condition, out int totalRow, int month=9999)
        {
            var date = DateTime.Now.AddMonths(-month);
            //TODO:前三个月数据a.CreatedDate>DateTime.Now.AddMonths(-3)
            var where = Repository.Entities.Where(a => a.LillyId == lillyid && a.MeetingTime > date)
                .GroupBy(x => x.MeetingCode)
                .Select(x => x.OrderByDescending(y => y.MeetingTime).ThenByDescending(y=>y.CreatedDate).FirstOrDefault());
             
            totalRow = where.Count();

            //var list = where.OrderBy(x=>x.Id).Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToList();//remove OrderByDescending(x=>x.ReceiveDate) by andrew
           var list = where.OrderByDescending(x => x.MeetingTime).Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToList();
            return list.Select(n => (Finance3rdQueryEntityView)new Finance3rdQueryEntityView().ConvertAPIModel(n)).ToList();
        }
    }
}
