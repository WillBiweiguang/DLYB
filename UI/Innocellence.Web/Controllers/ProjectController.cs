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

namespace Innocellence.Web.Controllers
{
    public class ProjectController : BaseController<Project, ProjectView>
    {
        private readonly IProjectService _projectService;
        private readonly IHistoricalCostService _objHistoricalCostService = new HistoricalCostService();
        public ProjectController(IProjectService projectService) : base(projectService)
        {
            _projectService = projectService;
        }
        // GET: Address
        public override ActionResult Index()
        {
            string projectId = Request["projectId"];
            ViewBag.ProjectId = projectId;
            var mode = Request["mode"];
            ViewBag.list= _objHistoricalCostService.GetList<HistoricalCostView>(int.MaxValue, x => !x.IsDeleted).ToList();
            ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            if(!string.IsNullOrEmpty(mode) && mode == "1") {
                ViewBag.Layout = "~/Views/Shared/_LayoutWithoutMenu.cshtml";
            }
            return View();
        }

        public override ActionResult GetList()
        {
            string projectId = Request["project_id"];
            int pid = string.IsNullOrEmpty(projectId) ? 0 : int.Parse(projectId);
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            Expression<Func<Project, bool>> expression = FilterHelper.GetExpression<Project>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition)&& pid!=0)
            {
                expression = expression.AndAlso<Project>(x => x.ProjectName.Contains(strCondition) && x.Id == pid && x.IsDeleted != true);
            }
            else if(pid != 0 &&string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<Project>(x => x.IsDeleted != true && x.Id == pid);
            }
            else if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<Project>(x => x.IsDeleted != true && x.ProjectName.Contains(strCondition));
            }
            else
            {
                expression = expression.AndAlso<Project>(x => x.IsDeleted != true);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<ProjectView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }

        public JsonResult GetDropdownList(string keyword = "")
        {           
            var list = _projectService.GetList<ProjectView>(int.MaxValue, x => !x.IsDeleted && x.ProjectName.Contains(keyword.Trim()))
    .Select(x => new { key = x.Id, value = x.ProjectName }).ToList();
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //public override ActionResult GetList()
        //{
        //    GridRequest gridRequest = new GridRequest(Request);
        //    string strCondition = Request["search_condition"];
        //    Expression<Func<Project, bool>> expression = FilterHelper.GetExpression<Project>(gridRequest.FilterGroup);
        //    if (!string.IsNullOrEmpty(strCondition))
        //    {
        //        expression = expression.AndAlso<Project>(x => x.WeldLocationType.Contains(strCondition));
        //    }
        //    int rowCount = gridRequest.PageCondition.RowCount;
        //    List<ProjectView> listEx = GetListEx(expression, gridRequest.PageCondition);
        //    return this.GetPageResult(listEx, gridRequest);
        //}
    }
}