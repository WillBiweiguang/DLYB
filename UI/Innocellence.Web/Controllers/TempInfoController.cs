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
    public class TempInfoController : BaseController<TempInfo, TempInfoView>
    {
        private const string SLASH = "/";
        private readonly ITempInfoService _TempInfoService;
        private readonly IProjectService _projectService;
        private readonly ITaskListService _taskListService;

        public TempInfoController(ITempInfoService TempInfoService, IProjectService projectService,
            ITaskListService taskListService) : base(TempInfoService)
        {
            _TempInfoService = TempInfoService;
            _projectService = projectService;
            _taskListService = taskListService;
        }
        // GET: Address
        public override ActionResult Index()
        {
            string projectId = Request["projectId"];
            ViewBag.ProjectId = projectId;
            return View();
        }

        public ActionResult GetList1()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];

            Expression<Func<TempInfo, bool>> expression = FilterHelper.GetExpression<TempInfo>(gridRequest.FilterGroup);


            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<TempInfo>(x => x.BeamName.Contains(strCondition) ||
                x.FileName.Contains(strCondition) || x.ProjectName.Contains(strCondition) || x.FigureNumber.Contains(strCondition));
            }

            int rowCount = gridRequest.PageCondition.RowCount;

            List<TempInfoView> listEx = GetListEx(expression, gridRequest.PageCondition);

            return this.GetPageResult(listEx, gridRequest);
        }
        public override ActionResult GetList()
        {
            string projectId = Request["projectId"];
            int pid = 0;
            string projectName = "";
            if (int.TryParse(projectId, out pid))
            {
                var project = _projectService.Repository.Entities.FirstOrDefault(x => x.Id == pid);
                projectName = project?.ProjectName;
            }
            GridRequest gridRequest = new GridRequest(Request);
            var list = new List<TempInfoView>();
            string strCondition = Request["search_condition"];
            if (!string.IsNullOrEmpty(strCondition))
            {
                if (!string.IsNullOrEmpty(projectName))
                {
                    list = _TempInfoService.Repository.Entities.
                                        Where(x => (x.BeamName.Contains(strCondition) ||
                                    x.FileName.Contains(strCondition) || x.FigureNumber.Contains(strCondition)) && x.ProjectName == projectName)
                                        .Select
                                      (x => new TempInfoView { BeamName = x.BeamName, FileName = x.FileName, ProjectName = x.ProjectName, FigureNumber = x.FigureNumber }).Distinct().
                                        OrderBy(x => x.BeamName).
                                      Skip(gridRequest.PageCondition.PageSize * gridRequest.PageCondition.RowCount).
                                      Take(gridRequest.PageCondition.PageSize).ToList();
                }
                else
                {
                    list = _TempInfoService.Repository.Entities.
                    Where(x => (x.BeamName.Contains(strCondition) ||
                x.FileName.Contains(strCondition) || x.FigureNumber.Contains(strCondition)))
                    .Select
                  (x => new TempInfoView { BeamName = x.BeamName, FileName = x.FileName, ProjectName = x.ProjectName, FigureNumber = x.FigureNumber }).Distinct().
                    OrderBy(x => x.BeamName).
                  Skip(gridRequest.PageCondition.PageSize * gridRequest.PageCondition.RowCount).
                  Take(gridRequest.PageCondition.PageSize).ToList();
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(projectName))
                {
                    list = _TempInfoService.Repository.Entities.Where(x => x.ProjectName == projectName).Select
                  (x => new TempInfoView { BeamName = x.BeamName, FileName = x.FileName, ProjectName = x.ProjectName, FigureNumber = x.FigureNumber }).Distinct().
                    OrderBy(x => x.BeamName).
                  Skip(gridRequest.PageCondition.PageSize * gridRequest.PageCondition.RowCount).
                  Take(gridRequest.PageCondition.PageSize).ToList();
                }
                else
                {
                    list = _TempInfoService.Repository.Entities.Select
                     (x => new TempInfoView { BeamName = x.BeamName, FileName = x.FileName, ProjectName = x.ProjectName, FigureNumber = x.FigureNumber }).Distinct().
                       OrderBy(x => x.BeamName).
                     Skip(gridRequest.PageCondition.PageSize * gridRequest.PageCondition.RowCount).
                     Take(gridRequest.PageCondition.PageSize).ToList();

                }
                    
            }
            return Json(new
            {
                sEcho = Request["draw"],
                iTotalRecords = gridRequest.PageCondition.RowCount,
                iTotalDisplayRecords = gridRequest.PageCondition.RowCount,
                aaData = list
            }, JsonRequestBehavior.AllowGet);
        }
    }
}