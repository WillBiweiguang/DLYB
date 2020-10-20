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
    public class BeamInfoController : BaseController<BeamInfo, BeamInfoView>
    {
        private const string SLASH = "/";
        private readonly IBeamInfoService _beamInfoService;
        private readonly IProjectService _projectService;
        private readonly ITaskListService _taskListService;

        public BeamInfoController(IBeamInfoService beamInfoService, IProjectService projectService,
            ITaskListService taskListService) : base(beamInfoService)
        {
            _beamInfoService = beamInfoService;
            _projectService = projectService;
            _taskListService = taskListService;
        }
        // GET: Address
        public override ActionResult Index()
        {
            string projectId = Request["projectId"];
            int pid = 0;
            if (int.TryParse(projectId, out pid))
            {
                var project = _projectService.Repository.Entities.FirstOrDefault(x => x.Id == pid);
                ViewBag.ProjectName = project?.ProjectName;
            }
            ViewBag.ProjectId = projectId;
            return View();
        }

        public JsonResult NextBeam(int projectId, int beamId)
        {
            var beam = _beamInfoService.GetList<BeamInfoView>(1, x => !x.IsDeleted && x.ProjectId == projectId && x.Id == beamId
            && x.ProcessStatus != (int)BeamProcessStatus.Complete).FirstOrDefault();
            if(beam == null)
            {
                return new JsonResult { Data = new { result = "failed", msg = "没有未完成的梁段" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult { Data = new { result = "success", data = beam.Id , data1=beam.ProjectId}, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            string projectId = Request["project_id"];
            int pid = string.IsNullOrEmpty(projectId) ? 0 : int.Parse(projectId);
            Expression<Func<BeamInfo, bool>> expression = FilterHelper.GetExpression<BeamInfo>(gridRequest.FilterGroup);
            expression = expression.AndAlso<BeamInfo>(x => x.IsDeleted != true);
            if (!string.IsNullOrEmpty(projectId))
            {
                expression = expression.AndAlso<BeamInfo>(x => x.ProjectId == pid );
            }
           if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<BeamInfo>(x => x.DwgFile.Contains(strCondition));
            }
            if (!string.IsNullOrEmpty(objLoginInfo.Department))
            {
                var departmentid = objLoginInfo.Department.Split('_').First();
                var projectIds = _projectService.GetList<ProjectView>(int.MaxValue, x => !x.IsDeleted && x.DepartmentID == departmentid).Select(x => x.Id).ToArray();
                expression = expression.AndAlso<BeamInfo>(x => projectIds.Contains(x.ProjectId));
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<BeamInfoView> listEx = GetListEx(expression, gridRequest.PageCondition);
            var projectIDs = listEx.Select(x => x.ProjectId).ToList();
            var projects = _projectService.GetList<ProjectView>(int.MaxValue, x => projectIDs.Contains(x.Id)).ToList();
            var tasklist = _taskListService.GetList<TaskListView>(int.MaxValue, x => !x.IsDeleted && projectIDs.Contains(x.ProjectId)).ToList();
            listEx.ForEach(w => {
                var p = projects.FirstOrDefault(x => x.Id == w.ProjectId);
                var task = tasklist.FirstOrDefault(x => x.ProjectId == w.ProjectId && (x.BeamId == w.Id || x.DWGFile == w.DwgFile));
                if (p != null)
                {
                    w.ProjectName = p.ProjectName;
                    w.TaskStatus = task == null ? 0 : task.TaskStatus;
                    w.Status = task == null ? ((BeamProcessStatus)w.ProcessStatus).ToDescription() : ((TaskStatus)task.TaskStatus).ToDescription();
                }
            });
            return this.GetPageResult(listEx, gridRequest);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PostFile(BeamInfoView objModal, int ProjectId)
        {
            //验证错误
            if (!ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    objModal = objModal ?? new BeamInfoView();
                    objModal.DwgFile = System.IO.Path.GetFileName(file.FileName);
                    objModal.ProjectId = ProjectId;
                    var project = _projectService.Repository.Entities.FirstOrDefault(x => x.Id == ProjectId);
                    if (project != null)
                    {
                        objModal.ProjectName = project.ProjectName;
                    }
                    var fileExtension = System.IO.Path.GetExtension(file.FileName);
                    if (fileExtension.ToLower() != ".dwg")
                    {
                        var result = GetErrorJson();
                        result.Message = new JsonMessage(103, "请上传dwg文件");
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    if (!System.IO.Directory.Exists(Server.MapPath("/Files/BeamInfo/" + ProjectId + SLASH)))
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("/Files/BeamInfo/" + ProjectId + SLASH));
                    }
                    string path = "/Files/BeamInfo/" + ProjectId + SLASH + objModal.DwgFile;
                    file.SaveAs(Server.MapPath(path));
                    if (!_beamInfoService.Repository.Entities.Any(x => x.ProjectId == ProjectId && x.DwgFile == objModal.DwgFile && !x.IsDeleted))
                    {
                        _beamInfoService.InsertView(objModal);
                    }
                }
            }
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PostDwgFile(int BeamId)
        {
            //验证错误
            if (!ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            if (Request.Files.Count > 0)
            {
                var beam = _beamInfoService.GetList<BeamInfoView>(1, x => x.Id == BeamId).FirstOrDefault();
                var file = Request.Files[0];
                string path = "/Files/BeamInfo/" + beam.ProjectId + SLASH + beam.DwgFile;
                file.SaveAs(Server.MapPath(path));
            }
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }
    }
}