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
            //var list = _addressService.GetList<AddressView>(int.MaxValue, x => !x.IsDeleted).ToList();
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

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            string projectId = Request["project_id"];
            int pid = string.IsNullOrEmpty(projectId) ? 0 : int.Parse(projectId);
            Expression<Func<BeamInfo, bool>> expression = FilterHelper.GetExpression<BeamInfo>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(projectId) && !string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<BeamInfo>(x => x.ProjectId == pid && x.IsDeleted != true && x.DwgFile.Contains(strCondition));
            }
            else if(!string.IsNullOrEmpty(projectId) && string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<BeamInfo>(x => x.IsDeleted != true && x.ProjectId == pid);
            }
            else if (!string.IsNullOrEmpty(strCondition)&& string.IsNullOrEmpty(projectId))
            {
                expression = expression.AndAlso<BeamInfo>(x => x.IsDeleted != true && x.DwgFile.Contains(strCondition));
            }
            else
            {
                expression = expression.AndAlso<BeamInfo>(x => x.IsDeleted != true);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<BeamInfoView> listEx = GetListEx(expression, gridRequest.PageCondition);
            var projectIDs = listEx.Select(x => x.ProjectId).ToList();
            var projects = _projectService.GetList<ProjectView>(int.MaxValue, x => projectIDs.Contains(x.Id)).ToList();
            var tasklist = _taskListService.GetList<TaskListView>(int.MaxValue, x => !x.IsDeleted && projectIDs.Contains(x.ProjectId)).ToList();
            listEx.ForEach(w => {
                var p = projects.FirstOrDefault(x => x.Id == w.ProjectId);
                var task = tasklist.FirstOrDefault(x => x.ProjectId == w.ProjectId && x.DWGFile == w.DwgFile);
                if(p!= null)
                {
                    w.ProjectName = p.ProjectName;
                    w.TaskStatus = task == null ? 0 : task.TaskStatus;
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
                var file = Request.Files[0];                
                objModal = objModal ?? new BeamInfoView();
                objModal.DwgFile = System.IO.Path.GetFileName(file.FileName);
                objModal.ProjectId = ProjectId;
                var fileExtension = System.IO.Path.GetExtension(file.FileName);
                if (fileExtension.ToLower() != ".dwg")
                {
                    var result = GetErrorJson();
                    result.Message = new JsonMessage(103, "请上传dwg文件");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                
                if (!System.IO.Directory.Exists(Server.MapPath("/Files/BeamInfo/"+ ProjectId+ SLASH)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("/Files/BeamInfo/" + ProjectId + SLASH));
                }
                string path = "/Files/BeamInfo/" + ProjectId + SLASH + objModal.DwgFile;
                file.SaveAs(Server.MapPath(path));
                if (!_beamInfoService.Repository.Entities.Any(x => x.ProjectId == ProjectId && x.DwgFile == objModal.DwgFile && !x.IsDeleted))
                {
                    _beamInfoService.InsertView(objModal);
                }
                //if (string.IsNullOrEmpty(Id) || Id == "0")
                //{
                //    _service.InsertView(objModal);
                //}
                //else
                //{
                //    _service.UpdateView(objModal);
                //}
            }
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }
    }
}