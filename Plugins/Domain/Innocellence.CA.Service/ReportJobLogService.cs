using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.ModelsView;

namespace DLYB.CA.Service
{
    public class ReportJobLogService : BaseService<ReportJobLogEntity>, IReportJobLogService
    {
       
        public List<ReportJobLogView> QueryJobLogListForView(Expression<Func<ReportJobLogEntity, bool>> func)
        {
            return Repository.Entities.Where(func).AsNoTracking().ToList().Select(
                n => (ReportJobLogView)(new ReportJobLogView().ConvertAPIModel(n))).ToList();
        }

        public IList<ReportJobLogEntity> QueryJobLogList(Expression<Func<ReportJobLogEntity, bool>> func)
        {
            return Repository.Entities.Where(func).AsNoTracking().ToList();
        }

        public IQueryable<ReportJobLogEntity> GetQueryable(Expression<Func<ReportJobLogEntity, bool>> func)
        {
            return Repository.Entities.Where(func).AsNoTracking();
        }

        public ReportJobLogEntity CreateJobLog(string jobName, ReportJobLogEntity entity = null)
        {
            //手动启动时创建
            if (entity != null)
            {
                Repository.Insert(entity);
                return entity;
            }

            var newEnity = new ReportJobLogEntity { JobStatus = JobStatus.Running.ToString(), JobName = jobName, CreatedDate = DateTime.Now };
            var success = JobStatus.Success.ToString();

            var jobEntity = GetQueryable(x => x.JobName == jobName && x.JobStatus == success).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

            if (jobEntity != null)
            {
                newEnity.DateFrom = jobEntity.DateTo;

                //TODO:check
                newEnity.DateTo = DateTime.Now.Date;
            }
            else
            {
                //1953
                newEnity.DateFrom = new DateTime(1953, 1, 1);
                newEnity.DateTo = DateTime.Now.Date;
            }

            Repository.Insert(newEnity);

            return newEnity;
        }

    }

    public enum JobStatus
    {
        Error,
        Running,
        Success,
    }
}
