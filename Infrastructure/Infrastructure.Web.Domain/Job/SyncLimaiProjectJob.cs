﻿using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Tasks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Web.Domain.Job
{
    public class SyncLimaiProjectJob : ITask
    {
        private readonly ILogger Logger;
        private readonly IProjectService _projectService;

        public SyncLimaiProjectJob(IProjectService projectService)
        {
            Logger = LogManager.GetLogger(this.GetType());
            _projectService = projectService;
        }

        public void Execute()
        {
            //var jobHour = ConfigurationManager.AppSettings["SyncLimaiHour"];
            //if (DateTime.Now.Hour != int.Parse(jobHour))
            //{
            //    return;
            //}
            Logger.Info("executing job start");
            string sql = @" Insert into t_ProjectInfo
(ProjectName,ProjectType,AffiliatedInstitution,Status,CreatedUserID,CreatedDate,
UpdatedUserID,UpdatedDate,IsDeleted,DepartmentID,CompleteDate,LmProjectId,LmAffiliatedId,
LmBridgeTypeId)
SELECT ProjectName,BridgeType,AffiliatedInstitution,'未完成',0 as C1,NOW(),0 AS C2,NOW(),
0 AS C3,0 AS C4,NULL,ProjectId,AffiliatedId,BridgeTypeId
FROM 
(select distinct projectId,ProjectName,AffiliatedInstitution,AffiliatedId,BridgeType,BridgeTypeId from t_TempInfo) AS T
WHERE NOT EXISTS(Select 1 From t_ProjectInfo P WHERE T.ProjectId = P.lmProjectId) 
";
            var result = _projectService.Repository.UnitOfWork.ExecuteSqlCommand(Core.TransactionalBehavior.DoNotEnsureTransaction, sql);
            string sqlUpdate = @"Update t_ProjectInfo P
INNER JOIN (select distinct projectId,ProjectName,AffiliatedInstitution,AffiliatedId,BridgeType,BridgeTypeId from t_TempInfo) AS T ON P.LmProjectId = T.projectId
set P.ProjectName = T.ProjectName, P.AffiliatedInstitution = T.AffiliatedInstitution, P.ProjectType = T.BridgeType,
P.LmAffiliatedId = T.AffiliatedId, P.LmBridgeTypeId = T.BridgeTypeId
WHERE P.ProjectName <> T.ProjectName OR P.AffiliatedInstitution <> T.AffiliatedInstitution OR P.ProjectType <> T.BridgeType OR
P.LmAffiliatedId <> T.AffiliatedId OR P.LmBridgeTypeId <> T.BridgeTypeId
";
           var result2 =  _projectService.Repository.UnitOfWork.ExecuteSqlCommand(Core.TransactionalBehavior.DoNotEnsureTransaction, sqlUpdate);
            //_projectService.Repository.UnitOfWork.SqlQuery<int>(sqlUpdate);
            Logger.Info("executing job end");
            Logger.Error(string.Format("insert {0}条" + "，Update{1}条", result, result2));
        }
    }
}
