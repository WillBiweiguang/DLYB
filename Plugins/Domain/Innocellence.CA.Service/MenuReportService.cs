using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using DLYB.CA.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.ViewModel;


namespace DLYB.CA.Service
{
    public class MenuReportService : BaseService<MenuReportEntity>, IMenuReportService
    {
        public List<MenuReportView> QueryTable(DateTime stardate, DateTime enddate, PageCondition pageCondition)
        {
            Expression<Func<MenuReportEntity, bool>> menuReport = a => a.AccessDate >= stardate && a.AccessDate <= enddate;

            return GetList<MenuReportView>(menuReport, pageCondition);
        }


        public IList<MenuReportEntity> QueryList(Expression<Func<MenuReportEntity, bool>> func)
        {
            return Repository.Entities.Where(func).ToList();
        }


        public List<T> GetListByDate<T>(Expression<Func<MenuReportEntity, bool>> predicate) where T : Infrastructure.Core.IViewModel, new()
        {
            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return lst;
        }
    }
}
