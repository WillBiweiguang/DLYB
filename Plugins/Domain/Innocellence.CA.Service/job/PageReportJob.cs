using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binbin.Linq;
using Infrastructure.Core;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.Logging;
using DLYB.CA.Contracts;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;

namespace DLYB.CA.Service.job
{
    public class PageReportJob : ICustomerJob
    {
        private readonly TimeSpan _interval;
        private readonly IReportJobLogService jobLogService = EngineContext.Current.Resolve<IReportJobLogService>();
        private readonly IUserBehaviorService userBehaviorService = EngineContext.Current.Resolve<IUserBehaviorService>();
        private readonly IPageReportService PageReportService = EngineContext.Current.Resolve<IPageReportService>();
        private static readonly ILogger log = LogManager.GetLogger(typeof(PageReportJob));
        private readonly string jobName = JobName.PageReportJob.ToString();//"PageReportJob";
        private bool _success = true;

        public PageReportJob()
        {
            _interval = TimeSpan.FromHours(24);
        }

        public Task Execute()
        {
            return new Task(RunJob);
        }

        private void RunJob()
        {
            ReportJobLogEntity jobLog = null;

            try
            {
                jobLog = jobLogService.CreateJobLog(jobName);
                ImportDataByTime(jobLog.DateFrom, jobLog.DateTo);
                jobLog.JobStatus = JobStatus.Success.ToString();
            }
            catch (DLYBException e)
            {
                if (jobLog != null)
                {
                    jobLog.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                    jobLog.JobStatus = JobStatus.Error.ToString();
                }

                log.Error<string>(string.Format("Page report job error:{0}" + Environment.NewLine + "{1}", e.Message, e.StackTrace));
            }
            catch (Exception e)
            {
                if (jobLog != null)
                {
                    jobLog.ErrorMessage = e.Message;
                    jobLog.JobStatus = JobStatus.Error.ToString();
                }

                log.Error<string>(string.Format("Page report job error:{0}" + Environment.NewLine + "{1}", e.Message, e.StackTrace));
            }
            finally
            {
                if (jobLog != null)
                {
                    jobLog.UpdatedDate = DateTime.Now;
                    jobLogService.Repository.Update(jobLog);
                }
            }
        }

        /// <summary>

        /// </summary>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        private void ImportDataByTime(DateTime? fromDateTime, DateTime toDateTime)
        {
            IList<PageReport> PageReportLogs = new List<PageReport>();
            var predicate = PredicateBuilder.True<UserBehavior>();

            predicate = predicate.And(x => x.CreatedTime <= toDateTime && x.CreatedTime > fromDateTime);

            //predicate = predicate.And(x =>x.Url!=""&& x.Url!=null);
            //predicate = predicate.And(x => x.ContentType != 9);
            predicate = predicate.And(x => x.ContentType == 0);
            var logs = userBehaviorService.Repository.Entities.Where(predicate).ToList();//.AsParallel();


            logs.GroupBy(x => x.FunctionId).ToList().ForEach(
                appGroup => appGroup.GroupBy(x => x.CreatedTime.Date).ToList().ForEach(appAccess =>
           {
               var appAccessAction = appAccess.FirstOrDefault();
               if (appAccessAction == null) return;

               var entity = new PageReport
               {
                   Appid = appAccessAction.AppId,
                   PageUrl = appAccessAction.FunctionId,
                   AccessDate = appAccessAction.CreatedTime.Date,
                   CreatedDate = DateTime.Now,
                   VisitTimes = appAccess.Count(),
                   VisitorCount = appAccess.GroupBy(x => x.UserId).Count()
               };
               PageReportLogs.Add(entity);
           }));
            PageReportService.Repository.Insert(PageReportLogs.AsEnumerable());
        }
        //            logs.GroupBy(x => x.AppId).ToList().ForEach(functionGroup => functionGroup.GroupBy(x => x.FunctionId).ToList().ForEach(
        //    appGroup => appGroup.GroupBy(x => x.CreatedTime.Date).ToList().ForEach(appAccess =>
        //{
        //    var appAccessAction = appAccess.FirstOrDefault();
        //    if (appAccessAction == null) return;
        //    var app =
        //        CommonService.lstSysWeChatConfig.FirstOrDefault(
        //            y => y.WeixinAppId == appAccessAction.AppId.ToString(CultureInfo.InvariantCulture));
        //    var appName = string.Empty;
        //    if (app == null)
        //    {
        //        log.Info("没有找到app id 为{0}的app!", appAccessAction.AppId);
        //    }
        //    else
        //    {
        //        appName = app.AppName;
        //    }
        //    var entity = new PageReport
        //    {
        //        Appid = appAccessAction.AppId,
        //        AppName = appName,
        //        PageUrl = appAccessAction.FunctionId,
        //        AccessDate = appAccessAction.CreatedTime.Date,
        //        CreatedDate = DateTime.Now,
        //        VisitTimes = appAccess.Count(),
        //        VisitorCount = appAccess.GroupBy(x => x.UserId).Count()
        //    };
        //    PageReportLogs.Add(entity);
        //})));
        //            PageReportService.Repository.Insert(PageReportLogs.AsEnumerable());
        //        }
        public TimeSpan Interval
        {
            get { return _interval; }
        }

        public string Name
        {
            get { return "Page Report Job"; }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.MaxValue; }
        }
        public void ManuallyRunJob()
        {
            RunJob();
        }

        public JobName JobName
        {
            get { return JobName.PageReportJob; }
        }
        public bool Success
        {
            get { return _success; }
        }
    }
}
