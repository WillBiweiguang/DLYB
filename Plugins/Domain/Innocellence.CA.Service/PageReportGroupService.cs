using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Extensions;
using DLYB.CA.Contracts;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace DLYB.CA.Services
{
    /// <summary>
    /// 业务实现——App访问量统计
    /// </summary>
    public partial class PageReportGroupService : BaseService<PageReportGroup>, IPageReportGroupService
    {
       
        public PageReportGroupService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            
        }

        public PageReportGroupService()
            
        {

        }

        public List<T> GetListByFromDate<T>(Expression<Func<PageReportGroup, bool>> predicate) where T : IViewModel, new()
        {
            //var lst = Repository.Entities.Where(predicate).DistinctBy(p => new { p.AccessDate }).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            var lst = Repository.Entities.Where(predicate).OrderByDescending(p => new { p.CreatedDate }).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return lst;
        }
          
    }
}