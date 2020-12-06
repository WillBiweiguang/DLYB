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
        private readonly IBeamInfoService _beamInfoService;

        public TempInfoController(ITempInfoService TempInfoService, IProjectService projectService,
            ITaskListService taskListService, IBeamInfoService beamInfoService) : base(TempInfoService)
        {
            _TempInfoService = TempInfoService;
            _projectService = projectService;
            _taskListService = taskListService;
            _beamInfoService = beamInfoService;
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
            //加入机构验证
            var department = objLoginInfo.Department.Split('_')[1];
            expression = expression.AndAlso<TempInfo>(x => x.AffiliatedInstitution == department);

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
            //加入机构验证
            var department = objLoginInfo.Department.Split('_')[1];
            
            GridRequest gridRequest = new GridRequest(Request);
            var list = new List<TempInfoView>();
            string strCondition = Request["search_condition"];
            if (!string.IsNullOrEmpty(strCondition))
            {
                if (!string.IsNullOrEmpty(projectName))
                {
                    list = _TempInfoService.Repository.Entities.Where(x=>x.AffiliatedInstitution == department).
                                        Where(x => (x.BeamName.Contains(strCondition) ||
                                    x.FileName.Contains(strCondition) || x.FigureNumber.Contains(strCondition)) && x.ProjectName == projectName)
                                        .Select
                                      (x => new TempInfoView { ProjectId = x.ProjectId, BeamName = x.BeamName, FileName = x.FileName, ProjectName = x.ProjectName, FigureNumber = x.FigureNumber }).Distinct().
                                        OrderBy(x => x.BeamName).
                                      Skip(gridRequest.PageCondition.PageSize * gridRequest.PageCondition.RowCount).
                                      Take(gridRequest.PageCondition.PageSize).ToList();
                }
                else
                {
                    list = _TempInfoService.Repository.Entities.Where(x => x.AffiliatedInstitution == department).
                    Where(x => (x.BeamName.Contains(strCondition) ||
                x.FileName.Contains(strCondition) || x.FigureNumber.Contains(strCondition)))
                    .Select
                  (x => new TempInfoView { ProjectId = x.ProjectId, BeamName = x.BeamName, FileName = x.FileName, ProjectName = x.ProjectName, FigureNumber = x.FigureNumber }).Distinct().
                    OrderBy(x => x.BeamName).
                  Skip(gridRequest.PageCondition.PageSize * gridRequest.PageCondition.RowCount).
                  Take(gridRequest.PageCondition.PageSize).ToList();
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(projectName))
                {
                    list = _TempInfoService.Repository.Entities.Where(x => x.AffiliatedInstitution == department).
                        Where(x => x.ProjectName == projectName).Select
                  (x => new TempInfoView { ProjectId = x.ProjectId, BeamName = x.BeamName, FileName = x.FileName, ProjectName = x.ProjectName, FigureNumber = x.FigureNumber }).Distinct().
                    OrderBy(x => x.BeamName).
                  Skip(gridRequest.PageCondition.PageSize * gridRequest.PageCondition.RowCount).
                  Take(gridRequest.PageCondition.PageSize).ToList();
                }
                else
                {
                    list = _TempInfoService.Repository.Entities.Where(x => x.AffiliatedInstitution == department).Select
                     (x => new TempInfoView { ProjectId = x.ProjectId, BeamName = x.BeamName, FileName = x.FileName, ProjectName = x.ProjectName, FigureNumber = x.FigureNumber }).Distinct().
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

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PostFile(string projectId, string beamName)
        {
            var tempInfo = _TempInfoService.GetList<TempInfoView>(1, x => x.ProjectId == projectId && x.BeamName == beamName).FirstOrDefault();
            var project = _projectService.GetList<ProjectView>(1, x => !x.IsDeleted && x.LmProjectId == tempInfo.ProjectId).FirstOrDefault();            
            if (project == null)
            {
                var result = GetErrorJson();
                result.Message = new JsonMessage(101, "该项目在系统中找不到");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var dwgfile = tempInfo.BeamName.ToLower() + ".dwg";
            var beam = _beamInfoService.GetList<BeamInfoView>(1, x => !x.IsDeleted && x.ProjectId == project.Id && x.DwgFile.ToLower() == dwgfile).FirstOrDefault();
            if (beam != null)
            {
                var result = GetErrorJson();
                result.Message = new JsonMessage(102, "梁段已上传，请勿重复上传");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var fileExtension = System.IO.Path.GetExtension(file.FileName);
                if (fileExtension.ToLower() != ".dwg")
                {
                    var result = GetErrorJson();
                    result.Message = new JsonMessage(103, "请上传dwg文件");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                var fileName = tempInfo.BeamName + ".dwg";
                var objModal = new BeamInfoView();
                objModal.DwgFile = fileName;
                objModal.ProjectId = project.Id;
                objModal.BeamNum = 1;
                objModal.ProjectName = project.ProjectName;

                if (!System.IO.Directory.Exists(Server.MapPath("/Files/BeamInfo/" + project.Id + SLASH)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("/Files/BeamInfo/" + project.Id + SLASH));
                }
                string path = "/Files/BeamInfo/" + project.Id + SLASH + Utility.ReplaceWebFileName(objModal.DwgFile);

                file.SaveAs(Server.MapPath(path));
                _beamInfoService.InsertView(objModal);

                if (project.Status != ProjectStauts.NotComplete)
                {
                    project.Status = ProjectStauts.NotComplete;
                    _projectService.UpdateView(project);
                }
            }

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

    }
}