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
        public readonly IBeamInfoService _beamInfoService;
        public readonly IWeldingService _weldingService;
        public TaskListController(ITaskListService TaskListService,
            IWeldCategoryLabelingService weldCategoryService,
            IWeldCategoryStatisticsVService wcsvService,
            IBeamInfoService beamInfoService,
            IWeldingService weldingService,
            ISysUserRoleService sysUserRoleService, IProjectService projectService,
            ISysUserService sysUserService) : base(TaskListService)
        {
            _TaskListService = TaskListService;
            _weldCategoryService = weldCategoryService;
            _wcsvService = wcsvService;
            _weldingService = weldingService;
            _beamInfoService = beamInfoService;
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
            // ViewBag.list = _weldCategoryService.Repository.Entities.Where(a => !a.IsDeleted).
            //Select(x=>x.BeamId).ToList();
            ViewBag.Stat = _wcsvService.Repository.Entities.Where(a => !a.IsDeleted).ToList();
            ViewBag.ThirdNav = "审核列表";
            return View();
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            string projectName = Request["project_name"];
            string taskStatus = Request["task_status"];
            int status = 0;
            int.TryParse(taskStatus, out status);
            Expression<Func<TaskList, bool>> expression = FilterHelper.GetExpression<TaskList>(gridRequest.FilterGroup);

            expression = expression.AndAlso<TaskList>(x => x.IsDeleted != true);
            //过滤机构
            if (!string.IsNullOrEmpty(objLoginInfo.Department))
            {
                var department = objLoginInfo.Department.Split('_')[1];
                var projectIds = _projectService.GetList<ProjectView>(int.MaxValue, x => !x.IsDeleted && x.AffiliatedInstitution == department).Select(x => x.Id).ToArray();
                expression = expression.AndAlso<TaskList>(x => projectIds.Contains(x.ProjectId));
            }
            //过滤角色
            if (objLoginInfo.Menus != null && !objLoginInfo.Menus.Any(x => x.Id == (int)EnumMenuId.TaskApprove))
            {
                expression = expression.AndAlso<TaskList>(x => x.CreatedUserID == objLoginInfo.Id);
            }
            //TODO 需要连其他表查询的，都需要改为视图提高速度。
            //文件名
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<TaskList>(x => x.DWGFile.Contains(strCondition));
            }
            //项目名
            if (!string.IsNullOrEmpty(projectName))
            {
                var pids = _projectService.GetList<ProjectView>(1000, x => !x.IsDeleted && x.ProjectName.Contains(projectName)).Select(x => x.Id).ToList();
                expression = expression.AndAlso<TaskList>(x => pids.Contains(x.ProjectId));
            }
            //状态
            if (status > 0)
            {
                expression = expression.AndAlso<TaskList>(x => x.TaskStatus == status);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<TaskListView> listEx = GetListEx(expression, gridRequest.PageCondition);
            var projectIDs = listEx.Select(x => x.ProjectId).ToList();
            var projects = _projectService.GetList<ProjectView>(int.MaxValue, x => projectIDs.Contains(x.Id)).ToList();
            var usersIds = listEx.Select(x => x.CreatedUserID).ToList();
            var users = _sysUserService.Repository.Entities.Where(x => usersIds.Contains(x.Id)).ToList();
            listEx.ForEach(w =>
            {
                var p = projects.FirstOrDefault(x => x.Id == w.ProjectId);
                if (p != null)
                {
                    w.ProjectName = p.ProjectName;
                }
                if (users.Any(u => u.Id == w.CreatedUserID))
                {
                    w.CreatedUserName = users.FirstOrDefault(u => u.Id == w.CreatedUserID).UserTrueName;
                }
            });
            return this.GetPageResult(listEx, gridRequest);
        }

        public ActionResult WeldingDetail(int? beamId)
        {
            ViewBag.BeamId = beamId;
            return View();
        }
        public ActionResult WeldingDownload(int? projectId)
        {
            ViewBag.ProjectId = "";
            if (projectId.HasValue)
            {
                ViewBag.ProjectId = projectId.ToString();
            }
            return View();
        }

        public ActionResult GetWeldingDetailList(int beamId)
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            Expression<Func<WeldCategoryLabeling, bool>> expression = FilterHelper.GetExpression<WeldCategoryLabeling>(gridRequest.FilterGroup);
            expression = expression.AndAlso<WeldCategoryLabeling>(x => x.IsDeleted != true && x.BeamId == beamId);
            int rowCount = gridRequest.PageCondition.RowCount;
            List<WeldCategoryLabelingView> listEx = _weldCategoryService.GetList<WeldCategoryLabelingView>(expression, gridRequest.PageCondition);
            listEx = TableListHelper.GenerateIndex(listEx, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }

        public ActionResult GetWeldingDownloadList(int? projectId)
        {
            GridRequest gridRequest = new GridRequest(Request);
            Expression<Func<WeldCategoryStatisticsV, bool>> expression = FilterHelper.GetExpression<WeldCategoryStatisticsV>(gridRequest.FilterGroup);
            expression = expression.AndAlso<WeldCategoryStatisticsV>(x => x.IsDeleted != true);
            if (projectId.HasValue && projectId.Value > 0)
            {
                expression = expression.AndAlso<WeldCategoryStatisticsV>(x => x.ProjectId == projectId);
            }
            IEnumerable<WeldCategoryStatisticsVView> queryList = GetBatchWeldingListQuery(ref expression);
            var listEx = queryList.Distinct().Skip((gridRequest.PageCondition.PageIndex - 1) * gridRequest.PageCondition.PageSize)
                .Take(gridRequest.PageCondition.PageSize).ToList();
            //List<WeldCategoryStatisticsVView> listEx = _wcsvService.GetList<WeldCategoryStatisticsVView>(expression, gridRequest.PageCondition);
            //GetWeldQuality(listEx);
            listEx = TableListHelper.GenerateIndex(listEx, gridRequest.PageCondition);
            gridRequest.PageCondition.RowCount = listEx.Count;
            return this.GetPageResult(listEx, gridRequest);
        }

        private IEnumerable<WeldCategoryStatisticsVView> GetBatchWeldingListQuery(ref Expression<Func<WeldCategoryStatisticsV, bool>> expression)
        {
            string strCondition = Request["search_condition"];
            if (!string.IsNullOrEmpty(objLoginInfo.Department))
            {
                var department = objLoginInfo.Department.Split('_')[1];
                expression = expression.AndAlso<WeldCategoryStatisticsV>(x => x.AffiliatedInstitution == department);
            }
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<WeldCategoryStatisticsV>(x => x.ProjectName.Contains(strCondition) || x.AddressName.Contains(strCondition) || x.WeldType.Contains(strCondition));
            }
            //首先根据项目、焊缝位置检索。然后根据厂址做分组
            var query = from s in _wcsvService.GetList<WeldCategoryStatisticsVView>(int.MaxValue, expression)
                        join b in _beamInfoService.GetList<BeamInfoView>(int.MaxValue, x => !x.IsDeleted)
                        on s.BeamId equals b.Id
                        join l in _weldCategoryService.GetList<WeldCategoryLabelingView>(int.MaxValue, x => !x.IsDeleted)
                        on new { s.BeamId, WeldLocation = s.WeldLocationType, s.WeldingModel }
                        equals new { l.BeamId, l.WeldLocation, WeldingModel = l.WeldingType }
                        group l by new
                        {
                            s.ProjectId,
                            s.ProjectName,
                            s.AddressName,
                            //l.WeldLocation,
                            //l.WeldType,
                            s.WeldingType,
                            s.WeldingSpecific,
                            s.WeldingUnit,
                            s.WeldingModel
                        } into g
                        select new WeldCategoryStatisticsVView
                        {
                            ProjectName = g.Key.ProjectName,
                            AddressName = g.Key.AddressName,
                            WeldingType = g.Key.WeldingType,
                            //WeldType = g.Key.WeldType,
                            WeldingModel = g.Key.WeldingModel,
                            WeldingSpecific = g.Key.WeldingSpecific,
                            WeldingUnit = g.Key.WeldingUnit,
                            //WeldLocationType = g.Key.WeldLocation,
                            Quality = g.Sum(x => x.WeldQuanlity)
                        };

            //var queryList = from s in query
            //                group s by new
            //                {
            //                    s.AffiliatedInstitution,
            //                    s.ProjectName,
            //                    s.AddressName,
            //                    s.WeldType,
            //                    s.WeldingModel,
            //                    s.WeldingSpecific,
            //                    s.WeldingUnit
            //                } into g
            //                select new WeldCategoryStatisticsVView
            //                {
            //                    AffiliatedInstitution = g.Key.AffiliatedInstitution,
            //                    ProjectName = g.Key.ProjectName,
            //                    AddressName = g.Key.AddressName,
            //                    WeldType = g.Key.WeldType,
            //                    WeldingModel = g.Key.WeldingModel,
            //                    WeldingSpecific = g.Key.WeldingSpecific,
            //                    WeldingUnit = g.Key.WeldingUnit,
            //                    Quality = g.Sum(x => x.Quality)
            //                };
            return query.OrderBy(x => x.ProjectName).ThenBy(x => x.AddressName).ThenBy(x => x.WeldType).ThenBy(x => x.WeldingModel);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PostFile(TaskListView objModal, int ProjectName)
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
                if (fileExtension.ToLower() != ".dwg" || fileExtension.ToLower() != ".dxf")
                {
                    var result = GetErrorJson();
                    result.Message = new JsonMessage(103, "请上传dwg/dxf文件");
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
        public ActionResult ExportToExcel(int beamId = 0, string Ids = "")
        {
            string fileName = "焊材_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";

            string templateFilename = Server.MapPath(templateExcelFilename);
            using (FileStream file = new FileStream(templateFilename, FileMode.Open, FileAccess.Read))
            {
                var workbook = new XSSFWorkbook(file);
                var sheet1 = workbook.GetSheet("焊材");

                // 导出 焊材详情表 
                //var answer = _pollingResultService.GetList(Id);
                var reportList1 = _weldCategoryService.Repository.Entities.Where(a => !a.IsDeleted && a.BeamId == beamId).ToList();
                if (!string.IsNullOrEmpty(Ids))
                {
                    var selectedIds = Ids.TrimEnd(',').Split(',');
                    reportList1 = _weldCategoryService.Repository.Entities.Where(a => !a.IsDeleted && a.BeamId == beamId && selectedIds.Contains(a.Id.ToString())).ToList();
                }
                int i = 1;
                string beamName = "";
                int projectId = 0;
                string projectName = "";
                foreach (var v in reportList1)
                {
                    var beam = _beamInfoService.Repository.Entities.FirstOrDefault(x => x.Id == v.BeamId);
                    if (beam.DwgFile.IndexOf("dwg") > 0)
                    {
                        beamName = beam.DwgFile.Substring(0, beam.DwgFile.IndexOf("dwg") - 1);
                        projectId = beam.ProjectId;
                        var project = _projectService.Repository.Entities.FirstOrDefault(x => x.Id == projectId);
                        projectName = project.ProjectName;
                    }
                    int j = 0;
                    var row = sheet1.CreateRow(i++);
                    row.CreateCell(j++).SetCellValue(i - 1);
                    row.CreateCell(j++).SetCellValue(projectName);
                    row.CreateCell(j++).SetCellValue(beamName);
                    row.CreateCell(j++).SetCellValue(v.FigureNumber);
                    row.CreateCell(j++).SetCellValue(v.BoardNumber);
                    row.CreateCell(j++).SetCellValue(v.WeldType);
                    row.CreateCell(j++).SetCellValue(v.Thickness);
                    row.CreateCell(j++).SetCellValue(v.WeldLocation);
                    row.CreateCell(j++).SetCellValue(v.SectionArea);
                    row.CreateCell(j++).SetCellValue(v.WeldLength);
                    //row.CreateCell(j++).SetCellValue(v.WeldingNumber);
                    row.CreateCell(j++).SetCellValue(v.Quantity);
                    row.CreateCell(j++).SetCellValue(v.BeamNum.HasValue ? v.BeamNum.Value : 0);
                    row.CreateCell(j++).SetCellValue(v.WeldQuanlity);
                    row.CreateCell(j++).SetCellValue(v.ConsumeFactor);
                    row.CreateCell(j++).SetCellValue(v.ConsumeFactor * v.WeldQuanlity);

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

        public ActionResult ExportToExcelDownload(int beamId = 0, string Ids = "", int? projectId = null)
        {
            string fileName = "批量焊材_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";

            string templateFilename = Server.MapPath(templateExcelFilenameDownload);
            using (FileStream file = new FileStream(templateFilename, FileMode.Open, FileAccess.Read))
            {
                var workbook = new XSSFWorkbook(file);
                var sheet1 = workbook.GetSheet("焊材");


                Expression<Func<WeldCategoryStatisticsV, bool>> expression = (x) => !x.IsDeleted;
                if (projectId.HasValue && projectId.Value > 0)
                {
                    expression = expression.AndAlso<WeldCategoryStatisticsV>(x => x.ProjectId == projectId);
                }
                IEnumerable<WeldCategoryStatisticsVView> queryList = GetBatchWeldingListQuery(ref expression);
                var reportList1 = queryList.ToList();
                if (!string.IsNullOrEmpty(Ids))
                {
                    var selectedIds = Ids.TrimEnd(',').Split(',');
                    reportList1 = TableListHelper.GenerateIndex(reportList1, new PageCondition { PageIndex = 1, PageSize = int.MaxValue });
                    reportList1 = reportList1.Where(x => selectedIds.Contains(x.Index.ToString())).ToList();
                }
                int i = 1;
                foreach (var v in reportList1)
                {
                    var welding = _weldingService.Repository.Entities.FirstOrDefault(x => x.WeldingModel == v.WeldingModel);

                    int j = 0;
                    var row = sheet1.CreateRow(i++);
                    row.CreateCell(j++).SetCellValue(i - 1);
                    row.CreateCell(j++).SetCellValue(v.ProjectName);
                    row.CreateCell(j++).SetCellValue(v.AddressName);
                    row.CreateCell(j++).SetCellValue(v.WeldingType);
                    row.CreateCell(j++).SetCellValue(v.WeldingModel);
                    if (welding != null)
                    {
                        row.CreateCell(j++).SetCellValue(welding.WeldingSpecific);
                        row.CreateCell(j++).SetCellValue(v.Quality);
                        row.CreateCell(j++).SetCellValue(welding.WeldingUnit);
                    }

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

        private void GetWeldQuality(List<WeldCategoryStatisticsVView> viewlist)
        {
            var beams = viewlist.Select(x => x.BeamId);
            //var query = _weldCategoryService.Repository.Entities.Where(x => !x.IsDeleted && beams.Contains(x.BeamId)).ToList();
            var query = from s in viewlist
                        join l in _weldCategoryService.Repository.Entities.Where(x => !x.IsDeleted)
                        on new { s.BeamId, WeldingModel = s.WeldingModel } equals new { l.BeamId, WeldingModel = l.WeldingType }
                        group l by new { s.ProjectName, s.AddressName, s.WeldingModel } into g
                        select new KeyValuePair<WeldCategoryStatisticsVView, double>(
                            new WeldCategoryStatisticsVView
                            {
                                ProjectName = g.Key.ProjectName,
                                AddressName = g.Key.AddressName,
                                WeldingModel = g.Key.WeldingModel
                            },
                            g.Sum(x => x.WeldQuanlity));
            var qualities = query.ToList();
            viewlist.ForEach(x =>
            {
                var item = qualities.FirstOrDefault(p => p.Key.WeldingModel == x.WeldingModel && p.Key.ProjectName == x.ProjectName && p.Key.AddressName == x.AddressName);
                x.Quality = item.Value;
            });
        }

        public ActionResult ApproveTask(int Id, int option)
        {
            var task = _TaskListService.GetList<TaskListView>(1, x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
            if (task == null || (task.TaskStatus != (int)TaskStatus.PendingApproval && task.TaskStatus != (int)TaskStatus.Rejected))
            {
                return new JsonResult { Data = new { result = ApiReturnCode.Fail, message = "任务不存在或状态不正常" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            //option = 1 审核，0 驳回
            task.TaskStatus = option == 1 ? (int)TaskStatus.Approved : (int)TaskStatus.Rejected;
            _TaskListService.UpdateView(task);

            //更新项目状态
            if (option == 1)
            {
                var project = _projectService.GetList<ProjectView>(1, x => !x.IsDeleted && x.Id == task.ProjectId).FirstOrDefault();
                if (project != null)
                {
                    _projectService.UpdateProjectStatus(project);
                }
            }
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