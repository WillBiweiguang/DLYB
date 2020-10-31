// -----------------------------------------------------------------------
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
            string sql = @"select p.ProjectName, p.DepartmentID, p.ProjectType as BridgeType, sum(t.WeldQuanlity) as HancaiTotal, t.WeldingType 
from t_weldcategorylabeling t
inner join t_BeamInfo b on t.BeamId = b.Id
inner join t_ProjectInfo p on b.ProjectId = p.Id
where t.IsDeleted <> 1 and b.IsDeleted <> 1 and p.IsDeleted <> 1 {0}
group by p.ProjectName,p.DepartmentID, p.ProjectType, t.WeldingType ";
            string whereCondition = "";
            if(!string.IsNullOrEmpty(query.DepartmentId))
            {
                whereCondition += string.Format(" and p.DepartmentID = '{0}'", query.DepartmentId);
            }
            if (!string.IsNullOrEmpty(query.ProjectName))
            {
                whereCondition += string.Format(" and p.ProjectName = '{0}'", query.ProjectName);
            }
            if (!string.IsNullOrEmpty(query.BridgeType))
            {
                whereCondition += string.Format(" and p.ProjectType = '{0}'", query.BridgeType);
            }
            if (query.StartDate.HasValue)
            {
                whereCondition += string.Format(" and p.CreatedDate > '{0}'", query.StartDate.Value.ToString("yyyy-MM-dd"));
            }
            if (query.EndDate.HasValue)
            {
                whereCondition += string.Format(" and p.CompleteDate < '{0}'", query.EndDate.Value.AddDays(1).ToString("yyyy-MM-dd"));
            }
            sql = string.Format(sql, whereCondition);
            return Repository.UnitOfWork.SqlQuery<HanjiProportionModel>(sql).ToList();
        }

        public IList<HanjiProportionModel> HancaiQuanlity(LiMaiApiViewModel query)
        {
            string sql = @"select p.ProjectName, p.DepartmentID, p.ProjectType as BridgeType, sum(t.WeldQuanlity) as HancaiTotal 
from t_weldcategorylabeling t
inner join t_BeamInfo b on t.BeamId = b.Id
inner join t_ProjectInfo p on b.ProjectId = p.Id
where t.IsDeleted <> 1 and b.IsDeleted <> 1 and p.IsDeleted <> 1 {0}
group by p.ProjectName,p.DepartmentID, p.ProjectType ";
            string whereCondition = "";
            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                whereCondition += string.Format(" and p.DepartmentID = '{0}'", query.DepartmentId);
            }
            if (!string.IsNullOrEmpty(query.ProjectName))
            {
                whereCondition += string.Format(" and p.ProjectName = '{0}'", query.ProjectName);
            }
            if (!string.IsNullOrEmpty(query.BridgeType))
            {
                whereCondition += string.Format(" and p.ProjectType = '{0}'", query.BridgeType);
            }
            if (query.StartDate.HasValue)
            {
                whereCondition += string.Format(" and p.CreatedDate > '{0}'", query.StartDate.Value.ToString("yyyy-MM-dd"));
            }
            if (query.EndDate.HasValue)
            {
                whereCondition += string.Format(" and p.CompleteDate < '{0}'", query.EndDate.Value.AddDays(1).ToString("yyyy-MM-dd"));
            }
            sql = string.Format(sql, whereCondition);
            return Repository.UnitOfWork.SqlQuery<HanjiProportionModel>(sql).ToList();
        }
    }
}