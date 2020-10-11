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
using Infrastructure.Utility.Data;
using System.IO;
using NPOI.XSSF.UserModel;
using Infrastructure.Web.Domain.Common;

namespace DLYB.Web.Controllers
{
    public class TaskListController : BaseController<TaskList, TaskListView>
    {
        private readonly ITaskListService _TaskListService;
        const string templateExcelFilename = "/content/焊材统计表.xlsx";
        const string templateExcelFilenameDownload = "/content/焊材批量统计表.xlsx";
        private readonly IWeldCategoryLabelingService _weldCategoryService;
        private readonly IWeldCategoryStatisticsVService _wcsvService;
        private readonly ISysUserRoleService _sysUserRoleService;
        private readonly IProjectService _projectService;
        private readonly ISysUserService _sysUserService;
        public TaskListController(ITaskListService TaskListService,
            IWeldCategoryLabelingService weldCategoryService,
            IWeldCategoryStatisticsVService wcsvService,
        ISysUserRoleService sysUserRoleService,IProjectService projectService,
            ISysUserService sysUserService) : base(TaskListService)
        {
            _TaskListService = TaskListService;
            _weldCategoryService = weldCategoryService;
            _wcsvService = wcsvService;
            _sysUserRoleService = sysUserRoleService;
            _projectService = projectService;
            _sysUserService = sysUserService;
        }
        // GET: Address
        public override ActionResult Index()
        {
            var role = objLoginInfo.Roles.FirstOrDefault(x => x.RoleId == 1);
            if (objLoginInfo.Roles.Count == 0)
            {
                role = _sysUserRoleService.Repository.Entities.Where(x => x.UserId == objLoginInfo.Id && x.RoleId == 1).FirstOrDefault();
            }            
            ViewBag.isApprover = role != null && role.RoleId == 1 ? 1 : 0;
            ViewBag.UserId = objLoginInfo.Id;
            ViewBag.list = _weldCategoryService.Repository.Entities.Where(a => !a.IsDeleted).ToList();
            ViewBag.Stat= _wcsvService.Repository.Entities.Where(a => !a.IsDeleted).ToList();
            return View();
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            Expression<Func<TaskList, bool>> expression = FilterHelper.GetExpression<TaskList>(gridRequest.FilterGroup);

            expression = expression.AndAlso<TaskList>(x => x.IsDeleted != true);

            int rowCount = gridRequest.PageCondition.RowCount;
            List<TaskListView> listEx = GetListEx(expression, gridRequest.PageCondition);
            var projectIDs = listEx.Select(x => x.ProjectId).ToList();
            var projects = _projectService.GetList<ProjectView>(int.MaxValue, x => projectIDs.Contains(x.Id)).ToList();
            var usersIds = listEx.Select(x => x.CreatedUserID).ToList();
            var users = _sysUserService.Repository.Entities.Where(x => usersIds.Contains(x.Id)).ToList();
            listEx.ForEach(w => {
                var p = projects.FirstOrDefault(x => x.Id == w.ProjectId);
                if (p != null)
                {
                    w.ProjectName = p.ProjectName;
                }
                if (users.Any(u => u.Id == w.CreatedUserID))
                {
                    w.CreatedUserName = users.FirstOrDefault(u => u.Id == w.CreatedUserID).UserName;
                }
            });
            return this.GetPageResult(listEx, gridRequest);
        }

        public ActionResult WeldingDetail(int? beamId)
        {
            ViewBag.BeamId = beamId;
            return View();
        }
        public ActionResult WeldingDownload()
        {
            return View();
        }

        public ActionResult GetWeldingDetailList(int beamId)
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            Expression<Func<WeldCategoryLabeling, bool>> expression = FilterHelper.GetExpression<WeldCategoryLabeling>(gridRequest.FilterGroup);
            expression = expression.AndAlso<WeldCategoryLabeling>(x => x.IsDeleted != true && x.BeamId == beamId);
            int rowCount = gridRequest.PageCondition.RowCount;
            List<WeldCategoryLabelingView> listEx = _weldCategoryService.GetList< WeldCategoryLabelingView>(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PostFile(TaskListView objModal,int ProjectName)
        {
            //验证错误
            if (!ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                objModal = objModal ?? new TaskListView();
                objModal.DWGFile = file.FileName;
                objModal.ProjectId = ProjectName;
                var fileExtension = System.IO.Path.GetExtension(file.FileName);
                if (fileExtension.ToLower() != ".dwg")
                {
                    var result = GetErrorJson();
                    result.Message = new JsonMessage(103, "请上传dwg文件");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (!System.IO.Directory.Exists(Server.MapPath("/Files/TaskList/")))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("/Files/TaskList/"));
                }
                string path = "/Files/TaskList/" + file.FileName;
                file.SaveAs(Server.MapPath(path));
                _TaskListService.InsertView(objModal);
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
        public ActionResult ExportToExcel(int beamId = 0)
        {
            string fileName =  "焊材_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";

            string templateFilename = Server.MapPath(templateExcelFilename);
            using (FileStream file = new FileStream(templateFilename, FileMode.Open, FileAccess.Read))
            {
                var workbook = new XSSFWorkbook(file);
                var sheet1 = workbook.GetSheet("焊材");

                // 导出 答卷 
                //var answer = _pollingResultService.GetList(Id);
                var reportList1 = _weldCategoryService.Repository.Entities.Where(a => !a.IsDeleted && a.BeamId == beamId).ToList();
                int i = 1;
                foreach (var v in reportList1)
                {
                    int j = 0;
                    var row = sheet1.CreateRow(i++);
                    row.CreateCell(j++).SetCellValue(i-1);
                    row.CreateCell(j++).SetCellValue(v.BeamId);
                    row.CreateCell(j++).SetCellValue(v.BeamId);
                    row.CreateCell(j++).SetCellValue(v.FigureNumber);
                    row.CreateCell(j++).SetCellValue(v.BoardNumber);
                    row.CreateCell(j++).SetCellValue(v.WeldType);
                    row.CreateCell(j++).SetCellValue(v.Thickness);
                    row.CreateCell(j++).SetCellValue(v.WeldLocation);
                    row.CreateCell(j++).SetCellValue(v.ConsumeFactor);
                    row.CreateCell(j++).SetCellValue(v.SectionArea);
                    row.CreateCell(j++).SetCellValue(v.WeldLength);
                    row.CreateCell(j++).SetCellValue(v.WeldQuanlity);
                    row.CreateCell(j++).SetCellValue(v.WeldingNumber);
                    row.CreateCell(j++).SetCellValue(v.Quantity);
                    row.CreateCell(j++).SetCellValue(v.BeamNum);
                    row.CreateCell(j++).SetCellValue(v.ConsumeFactor*v.WeldQuanlity);

                }
                
                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    ms.Flush();
                    ms.Position = 0;
                    //sheet = null;
                    //workbook = null;
                    //workbook.Close();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet
                    return File(ms.ToArray(), "application/vnd-excel", fileName);
                }

            }

        }
        public ActionResult ExportToExcelDownload(int beamId = 0)
        {
            string fileName = "批量焊材_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";

            string templateFilename = Server.MapPath(templateExcelFilenameDownload);
            using (FileStream file = new FileStream(templateFilename, FileMode.Open, FileAccess.Read))
            {
                var workbook = new XSSFWorkbook(file);
                var sheet1 = workbook.GetSheet("焊材");
                
                //var answer = _pollingResultService.GetList(Id);
                var reportList1 = _wcsvService.GetList<WeldCategoryStatisticsVView>(1,x => !x.IsDeleted ).ToList();
                int i = 1;
                foreach (var v in reportList1)
                {
                    int j = 0;
                    var row = sheet1.CreateRow(i++);
                    row.CreateCell(j++).SetCellValue(i - 1);
                    row.CreateCell(j++).SetCellValue(v.BeamId);
                    row.CreateCell(j++).SetCellValue(v.AddressName);
                    row.CreateCell(j++).SetCellValue(v.WeldType);
                    row.CreateCell(j++).SetCellValue(v.WeldingModel);
                    row.CreateCell(j++).SetCellValue(v.SectionalArea);

                }

                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    ms.Flush();
                    ms.Position = 0;
                    //sheet = null;
                    //workbook = null;
                    //workbook.Close();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet
                    return File(ms.ToArray(), "application/vnd-excel", fileName);
                }

            }

        }
        public ActionResult ApproveTask(int Id, int option) {
            var task = _TaskListService.GetList<TaskListView>(1, x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
            if (task == null || (task.TaskStatus != (int)TaskStatus.PendingApproval && task.TaskStatus != (int)TaskStatus.Rejected))
            {
                return new JsonResult { Data = new { result = ApiReturnCode.Fail, message = "任务不存在或状态不正常" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            //option = 1 审核，0 驳回
            task.TaskStatus = option == 1 ? (int)TaskStatus.Approved : (int)TaskStatus.Rejected;
            _TaskListService.UpdateView(task);
            return new JsonResult { Data = new { result = ApiReturnCode.Success }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult RaiseApprove(int Id)
        {
            var task = _TaskListService.GetList<TaskListView>(1, x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
            if (task == null || (task.TaskStatus != (int)TaskStatus.NotRequest && task.TaskStatus != (int)TaskStatus.Rejected))
            {
                return new JsonResult { Data = new { result = ApiReturnCode.Fail, message = "任务不存在或状态不正常" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            task.TaskStatus = (int)TaskStatus.PendingApproval;
            _TaskListService.UpdateView(task);
            return new JsonResult { Data = new { result = ApiReturnCode.Success }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}