using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Core;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.ModelsView;

namespace DLYB.CA.Contracts.Contracts
{
    public interface IReportJobLogService : IDependency, IBaseService<ReportJobLogEntity>
    {
        List<ReportJobLogView> QueryJobLogListForView(Expression<Func<ReportJobLogEntity, bool>> func);

        IList<ReportJobLogEntity> QueryJobLogList(Expression<Func<ReportJobLogEntity, bool>> func);

        IQueryable<ReportJobLogEntity> GetQueryable(Expression<Func<ReportJobLogEntity, bool>> func);

        ReportJobLogEntity CreateJobLog(string jobName, ReportJobLogEntity entity = null);
    }
}
