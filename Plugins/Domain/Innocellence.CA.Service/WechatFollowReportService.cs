using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using DLYB.CA.Contracts;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace DLYB.CA.Services
{
    public partial class WechatFollowReportService : BaseService<WechatFollowReport>, IWechatFollowReportService
    {
        public WechatFollowReportService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            
        }

        public WechatFollowReportService()           
        {

        }

        public List<T> GetListByFromStatisticsDate<T>(Expression<Func<WechatFollowReport, bool>> predicate) where T : IViewModel, new()
        {
            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return lst;
        }
                
    }
}