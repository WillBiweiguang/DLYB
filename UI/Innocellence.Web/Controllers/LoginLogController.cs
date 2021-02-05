using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Model;
using Innocellence.Web.Models;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.UI;
using Infrastructure.Utility.Filter;
using System.Linq.Expressions;
using DLYB.Web.Controllers;
using Infrastructure.Web.Domain.Services;
using Infrastructure.Web.Domain.Common;
using Infrastructure.Utility.Extensions;

namespace Innocellence.Web.Controllers
{
    public class LoginLogController : BaseController<LoginLog, LoginLogView>
    {
        private const string SLASH = "/";
        private readonly ILoginLogService _loginLogService;
        private readonly IBeamInfoService _beamInfoService;
        private readonly IProjectService _projectService;
        private readonly ITaskListService _taskListService;
        private readonly ITempInfoService _tempInfoService;

        public LoginLogController(ILoginLogService loginLogService, IBeamInfoService beamInfoService, IProjectService projectService,
            ITaskListService taskListService, ITempInfoService tempInfoService) : base(loginLogService)
        {
            _loginLogService = loginLogService;
            _beamInfoService = beamInfoService;
            _projectService = projectService;
            _taskListService = taskListService;
            _tempInfoService = tempInfoService;
        }
        // GET: Address
        public override ActionResult Index()
        {
            ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            if (!string.IsNullOrEmpty(objLoginInfo.Department))
            {
                ViewBag.departmentId = objLoginInfo.Department.Split('_')[1];
            }
            ViewBag.ThirdNav = "登录日志";
            return View();
        }

        public override ActionResult GetList()
        {            
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            
            Expression<Func<LoginLog, bool>> expression = FilterHelper.GetExpression<LoginLog>(gridRequest.FilterGroup);
            expression = expression.AndAlso<LoginLog>(x => x.IsDeleted != true);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<LoginLog>(x => x.UserName == strCondition || x.UserTrueName == strCondition);
            }  
            
            if (!string.IsNullOrEmpty(objLoginInfo.Department) && !Infrastructure.Core.Infrastructure.EngineContext.Current.WebConfig.SupperUser.Contains(objLoginInfo.UserName))
            {
                var department = objLoginInfo.Department.Split('_')[1];
                expression = expression.AndAlso<LoginLog>(x => x.AffiliatedInstitution == department);
            }

            int rowCount = gridRequest.PageCondition.RowCount;
            List<LoginLogView> listEx = GetListEx(expression, gridRequest.PageCondition);           
            return this.GetPageResult(listEx, gridRequest);
        }

    }
}