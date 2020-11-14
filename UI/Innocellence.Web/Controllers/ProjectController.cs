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
using Newtonsoft.Json;
using Infrastructure.Web.Domain.Common;
using Infrastructure.Web.Domain.Service;

namespace Innocellence.Web.Controllers
{
    public class ProjectController : BaseController<Project, ProjectView>
    {
        private readonly IProjectService _projectService;
        private readonly IHistoricalCostService _objHistoricalCostService = new HistoricalCostService();
        private readonly ISysUserService _sysUserService;
        readonly IList<KeyValuePair<int, string>> Departments;
        public ProjectController(IProjectService projectService, ISysUserService sysUserService) : base(projectService)
        {
            _projectService = projectService;
            _sysUserService = sysUserService;
            var deparments = CommonService.GetSysConfig(Consts.DepartmentConfigKey, Consts.DefaultDepartments);
            Departments = JsonConvert.DeserializeObject<List<KeyValuePair<int, string>>>(deparments);
        }
        // GET: Address
        public override ActionResult Index()
        {
            string projectId = Request["projectId"];
            ViewBag.ProjectId = projectId;
            var mode = Request["mode"];
            if (!string.IsNullOrEmpty(objLoginInfo.Department))
            {
                ViewBag.departmentId = objLoginInfo.Department.Split('_')[1];
            }
            ViewBag.list = _objHistoricalCostService.GetList<HistoricalCostView>(int.MaxValue, x => !x.IsDeleted).ToList();
            ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            if (!string.IsNullOrEmpty(mode) && mode == "1")
            {
                ViewBag.Layout = "~/Views/Shared/_LayoutWithoutMenu.cshtml";
            }
            ViewBag.ThirdNav = "项目管理";
            return View();
        }

        public override ActionResult GetList()

        {
            string projectId = Request["project_id"];
            int pid = 0;
            

            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];

            Expression<Func<Project, bool>> expression = FilterHelper.GetExpression<Project>(gridRequest.FilterGroup);
            expression = expression.AndAlso<Project>(x => x.IsDeleted != true);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<Project>(x => x.ProjectName.Contains(strCondition));
            }
            if (int.TryParse(projectId, out pid) && pid > 0)
            {
                expression = expression.AndAlso<Project>(x => x.Id == pid);
            }

            if (!string.IsNullOrEmpty(objLoginInfo.Department))
            {
                var department = objLoginInfo.Department.Split('_')[1];
                expression = expression.AndAlso<Project>(x => x.AffiliatedInstitution == department);
            }

            int rowCount = gridRequest.PageCondition.RowCount;
            List<ProjectView> listEx = GetListEx(expression, gridRequest.PageCondition);
            var userIds = listEx.Select(x => x.UpdatedUserID).ToArray();
            var users = _sysUserService.GetList<SysUserView>(10, x => userIds.Contains(x.Id)).ToList();
            listEx.ForEach(x =>
            {
                x.DepartmentName = x.AffiliatedInstitution;
                x.UpdateUserName = users.FirstOrDefault(e => e.Id == x.UpdatedUserID)?.UserTrueName;
            });
            return this.GetPageResult(listEx, gridRequest);
        }

        public JsonResult GetDropdownList(string keyword = "")
        {
            Expression<Func<Project, bool>> expression = x => !x.IsDeleted && x.ProjectName.Contains(keyword.Trim());
            if (!string.IsNullOrEmpty(objLoginInfo.Department))
            {
                var department = objLoginInfo.Department.Split('_')[1];
                expression = expression.AndAlso<Project>(x => x.AffiliatedInstitution == department);
            }
            var list = _projectService.GetList<ProjectView>(int.MaxValue, expression)
        .Select(x => new { key = x.Id, value = x.ProjectName }).ToList();
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}