﻿// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 17:21</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;


using System.Data.Entity.Infrastructure;
using System.Globalization;

using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using Infrastructure.Web.Domain.Service.Common;
using System.Security.Principal;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Common;
using TaskStatus = Infrastructure.Web.Domain.Common.TaskStatus;

namespace Infrastructure.Web.Domain.Services
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ProjectService :  BaseService<Project>, IProjectService
    {
        private readonly IWeldCategoryLabelingService _weldCategoryService;
        private readonly IBeamInfoService _beamInfoService;
        private readonly ITaskListService _taskListService;
        public ProjectService(IWeldCategoryLabelingService weldCategoryLabelingService
            , IBeamInfoService beamInfoService
            , ITaskListService taskListService)
        {
            _weldCategoryService = weldCategoryLabelingService;
            _beamInfoService = beamInfoService;
            _taskListService = taskListService;
        }

        public IList<HanjiProportionModel> HancaiQuanlityByType(LiMaiApiViewModel query)
        {
            string sql = @"select p.ProjectName, p.LmProjectId AS ProjectId, p.AffiliatedInstitution as Department, p.LmAffiliatedId AS AffiliatedId,  
p.ProjectType as BridgeType, p.LmBridgeTypeId as BridgeTypeId, sum(b.BeamNum * t.ConsumeFactor * t.WeldQuanlity / 100) as HanjiTotal, 
sum(b.BeamNum * t.WeldQuanlity) as HancaiTotal, t.WeldingType as HanjiType
from t_weldcategorylabeling t
inner join t_BeamInfo b on t.BeamId = b.Id
inner join t_ProjectInfo p on b.ProjectId = p.Id
where t.IsDeleted <> 1 and b.IsDeleted <> 1 and p.IsDeleted <> 1 {0}
group by p.ProjectName,p.LmProjectId, p.AffiliatedInstitution,p.LmAffiliatedId, p.ProjectType, p.LmBridgeTypeId, t.WeldingType ";
            string whereCondition = "";
            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                whereCondition += string.Format(" and (p.LmAffiliatedId = '{0}' )", query.DepartmentId);
            }
            if (!string.IsNullOrEmpty(query.ProjectId))
            {
                whereCondition += string.Format(" and p.LmProjectId = '{0}'", query.ProjectId);
            }
            if (!string.IsNullOrEmpty(query.BridgeTypeId))
            {
                whereCondition += string.Format(" and p.LmBridgeTypeId = '{0}'", query.BridgeTypeId);
            }
            if (query.StartDate.HasValue)
            {
                whereCondition += string.Format(" and p.CompleteDate >= '{0}'", query.StartDate.Value.ToString("yyyy-MM-dd"));
            }
            if (query.EndDate.HasValue)
            {
                whereCondition += string.Format(" and p.CompleteDate <= '{0}'", query.EndDate.Value.AddDays(1).ToString("yyyy-MM-dd"));
            }
            sql = string.Format(sql, whereCondition);
            return Repository.UnitOfWork.SqlQuery<HanjiProportionModel>(sql).ToList();
        }

        public IList<HanjiProportionModel> HancaiQuanlity(LiMaiApiViewModel query)
        {
            string sql = @"select p.ProjectName, p.AffiliatedInstitution as Department , p.ProjectType as BridgeType, p.LmProjectId, p.LmAffiliatedId, p.LmBridgeTypeId,
sum(b.BeamNum * t.ConsumeFactor  * t.WeldQuanlity / 100) as HanjiTotal, sum(b.BeamNum * t.WeldQuanlity) as HancaiTotal 
from t_weldcategorylabeling t
inner join t_BeamInfo b on t.BeamId = b.Id
inner join t_ProjectInfo p on b.ProjectId = p.Id
where t.IsDeleted <> 1 and b.IsDeleted <> 1 and p.IsDeleted <> 1 {0}
group by p.ProjectName,p.AffiliatedInstitution, p.ProjectType,p.LmProjectId,p.LmAffiliatedId,p.LmBridgeTypeId";
            string whereCondition = "";
            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                whereCondition += string.Format(" and (p.LmAffiliatedId = '{0}' )", query.DepartmentId);
            }
            if (!string.IsNullOrEmpty(query.ProjectId))
            {
                whereCondition += string.Format(" and p.LmProjectId = '{0}'", query.ProjectId);
            }
            if (!string.IsNullOrEmpty(query.BridgeTypeId))
            {
                whereCondition += string.Format(" and p.LmBridgeTypeId = '{0}'", query.BridgeTypeId);
            }
            if (query.StartDate.HasValue)
            {
                whereCondition += string.Format(" and p.CompleteDate >= '{0}'", query.StartDate.Value.ToString("yyyy-MM-dd"));
            }
            if (query.EndDate.HasValue)
            {
                whereCondition += string.Format(" and p.CompleteDate <= '{0}'", query.EndDate.Value.AddDays(1).ToString("yyyy-MM-dd"));
            }
            sql = string.Format(sql, whereCondition);
            return Repository.UnitOfWork.SqlQuery<HanjiProportionModel>(sql).ToList();
        }

        public void UpdateProjectStatus(ProjectView project)
        {
            //确认是否完成项目
            if (!_beamInfoService.Repository.Entities.Any(x => !x.IsDeleted && x.ProjectId == project.Id && x.ProcessStatus != (int)BeamProcessStatus.Complete)
                 && !_taskListService.Repository.Entities.Any(x => !x.IsDeleted && x.ProjectId == project.Id && x.TaskStatus != (int)TaskStatus.Approved)
                 && _beamInfoService.Repository.Entities.Any(x => !x.IsDeleted && x.ProjectId == project.Id))
            {
                project.Status = ProjectStauts.Complete;
                UpdateView(project);
            }
            else if (_beamInfoService.Repository.Entities.Any(x => !x.IsDeleted && x.ProjectId == project.Id && x.ProcessStatus != (int)BeamProcessStatus.Complete)
                 || _taskListService.Repository.Entities.Any(x => !x.IsDeleted && x.ProjectId == project.Id && x.TaskStatus != (int)TaskStatus.Approved))
            {
                project.Status = ProjectStauts.NotComplete;
                UpdateView(project);
            }
        }

        public void UpdateProjectStatus(int projectId)
        {
            var project = GetList<ProjectView>(1, x => !x.IsDeleted && x.Id == projectId).FirstOrDefault();
            if (project != null)
            {
                UpdateProjectStatus(project);
            }
        }

        public void DeleteProject(int projectId)
        {
            var project = GetList<ProjectView>(1, x => !x.IsDeleted && x.Id == projectId).FirstOrDefault();
            var sql = string.Format(@"update t_ProjectInfo set isdeleted = 1 where id = {0} ; ", projectId);
            var sqlbeam = string.Format("update t_BeamInfo set isdeleted = 1 where ProjectId = {0};", projectId);
            var sqlTask = string.Format(@"update t_TaskList set isdeleted = 1 where ProjectId = {0}; 
update t_weldcategorylabeling set IsDeleted = 1 where BeamId in (select id from t_BeamInfo where projectid = {0}) ;
update t_weldcategorystatistics set IsDeleted = 1 where BeamId in (select id from t_BeamInfo where projectid = {0}) ;", projectId);
            var sqldelete = "";
            if (!string.IsNullOrEmpty(project.LmProjectId))
            {
                sqldelete = string.Format("delete from t_TempInfo where ProjectId = '{0}';", project.LmProjectId);
            }
            Repository.UnitOfWork.ExecuteSqlCommand(Core.TransactionalBehavior.DoNotEnsureTransaction, sql);
            Repository.UnitOfWork.ExecuteSqlCommand(Core.TransactionalBehavior.DoNotEnsureTransaction, sqlbeam);
            Repository.UnitOfWork.ExecuteSqlCommand(Core.TransactionalBehavior.DoNotEnsureTransaction, sqlTask);
            if (!string.IsNullOrEmpty(sqldelete))
            {
                Repository.UnitOfWork.ExecuteSqlCommand(Core.TransactionalBehavior.DoNotEnsureTransaction, sqldelete);
            }
        }
    }
}