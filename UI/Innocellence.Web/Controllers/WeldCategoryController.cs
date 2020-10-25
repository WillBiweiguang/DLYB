using DLYB.Web.Controllers;
using Infrastructure.Utility.Filter;
using Infrastructure.Web.Domain.Common;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Innocellence.FaultSearch.Controllers
{
    public class WeldCategoryController : BaseController<WeldCategoryLabeling, WeldCategoryLabelingView>
    {
        private readonly IWeldCategoryLabelingService _weldCategoryService;
        private readonly IBeamInfoService _beamInfoService;
        private readonly IWeldGeometryService _weldGeometryService;
        private readonly IWeldLocationService _weldLocationService;
        private readonly ITaskListService _taskListService;
        private readonly IWeldCategoryStatisticsVService _weldCategoryStatisticsVService;
        private readonly IProjectService _projectService;
        private readonly IBoardPackageService _boardPackageService;

        private readonly string baseUrl = "http://42.202.130.245:3001/";
        public WeldCategoryController(IWeldCategoryLabelingService weldCategoryService,
            IBeamInfoService beamInfoService, IWeldGeometryService weldGeometryService,
            IWeldLocationService weldLocationService, ITaskListService taskListService,
            IBoardPackageService boardPackageService,
        IWeldCategoryStatisticsVService weldCategoryStatisticsVService,IProjectService projectService) : base(weldCategoryService)
        {
            _weldCategoryService = weldCategoryService;
            _beamInfoService = beamInfoService;
            _weldGeometryService = weldGeometryService;
            _weldLocationService = weldLocationService;
            _boardPackageService = boardPackageService;
            baseUrl = ConfigurationManager.AppSettings["WebUrl"];
            _taskListService = taskListService;
            _weldCategoryStatisticsVService = weldCategoryStatisticsVService;
            _projectService = projectService;
        }
        public override ActionResult Index()
        {
            int beamId = -1;
            if (int.TryParse(Request["beamId"], out beamId))
            {
                var beam = _beamInfoService.GetList<BeamInfoView>(1, x => x.Id == beamId).FirstOrDefault();
                if (beam != null)
                {
                    ViewBag.BeamId = beamId;
                    ViewBag.BeamName = beam.DwgFile;
                    ViewBag.ProjectId = beam.ProjectId;
                    ViewBag.ProjectName = string.IsNullOrEmpty(beam.ProjectName) ? _projectService.Repository.Entities.FirstOrDefault(x => x.Id == beam.ProjectId).ProjectName
                        : beam.ProjectName;
                    ViewBag.FileName = beam.DwgFile;
                    ViewBag.FilePath = GetFilePath(beam.ProjectId, beam.DwgFile);
                    ViewBag.FileServerPath = GetFileAbsolutePath(beam.ProjectId, beam.DwgFile);
                    var isView = Request["viewMode"];
                    ViewBag.ViewModel = 0;
                    if (!string.IsNullOrEmpty(isView) && isView == "1")
                    {
                        ViewBag.ViewModel = 1;
                    }
                    else if (beam.ProcessStatus == (int)BeamProcessStatus.Complete)
                    {
                        var task = _taskListService.GetList<TaskListView>(1, x => !x.IsDeleted && x.ProjectId == beam.ProjectId && x.BeamId == beam.Id).FirstOrDefault();
                        if (task == null || task.TaskStatus == (int)TaskStatus.NotRequest || task.TaskStatus == (int)TaskStatus.Rejected)
                        {
                            ViewBag.ViewModel = 0;
                        }
                    }
                }
            }
            var statistics = _weldCategoryStatisticsVService.GetList<WeldCategoryStatisticsVView>(int.MaxValue, x => !x.IsDeleted && x.BeamId == beamId).ToList();
            ViewBag.Figures = _weldCategoryService.Repository.Entities.Select(x => x.FigureNumber).Distinct().Select(x => new SelectListItem { Value = x, Text = x }).ToList();
            ViewBag.Boards = _weldCategoryService.Repository.Entities.Select(x => x.BoardNumber).Distinct().Select(x => new SelectListItem { Value = x, Text = x }).ToList();
            ViewBag.Areas = statistics.Select(x => x.SectionalArea).Distinct().ToList();
            ViewBag.weldGeometries = statistics.Select(x => x.WeldType).Distinct().ToList();
            ViewBag.weldLocations = statistics.Select(x => x.WeldLocationType).Distinct().ToList();
            return View();
        }

        //弹出层列表，目前暂时没有用
        public ActionResult WeldDataTable(int beamId = 0)
        {
            if (beamId > 0)
            {
                var beam = _beamInfoService.GetList<BeamInfoView>(1, x => x.Id == beamId).FirstOrDefault();
                if (beam != null)
                {
                    ViewBag.BeamId = beamId;
                    ViewBag.ProjectId = beam.ProjectId;
                    ViewBag.FileName = beam.DwgFile;
                }
            }
            ViewBag.weldCategorys = _weldCategoryService.Repository.Entities.Where(a => !a.IsDeleted).ToList();
            ViewBag.weldGeometries = _weldGeometryService.Repository.Entities.Where(x => !x.IsDeleted).ToList();
            ViewBag.weldLocations = _weldLocationService.Repository.Entities.Where(x => !x.IsDeleted).ToList();
            return View();
        }
        //弹出层编辑
        public override ActionResult Edit(string Id)
        {
            WeldCategoryLabelingView model = new WeldCategoryLabelingView();
            if (!string.IsNullOrEmpty(Id) && Id != "0" && Id != "1")
            {
                if (Id.Contains(','))
                {
                    var firstId = Id.TrimEnd(',').Split(',').First(x => !string.IsNullOrEmpty(x));
                    model = GetObject(firstId);
                    model.Ids = Id.TrimEnd(',');
                }
                else
                {
                    model = GetObject(Id);
                }
                var beam = _beamInfoService.GetList<BeamInfoView>(1, x => x.Id == model.BeamId).FirstOrDefault();
                if (beam != null)
                {
                    ViewBag.BeamId = beam.Id;
                    ViewBag.ProjectId = beam.ProjectId;
                    ViewBag.FileName = beam.DwgFile;
                    var statistics = _weldCategoryStatisticsVService.GetList<WeldCategoryStatisticsVView>(int.MaxValue, x => !x.IsDeleted && x.BeamId == beam.Id).ToList();
                    ViewBag.Figures = _weldCategoryService.Repository.Entities.Select(x => x.FigureNumber).Distinct().Select(x => new SelectListItem { Value = x, Text = x }).ToList();
                    ViewBag.Boards = _weldCategoryService.Repository.Entities.Select(x => x.BoardNumber).Distinct().Select(x => new SelectListItem { Value = x, Text = x }).ToList();
                    ViewBag.Areas = statistics.Select(x => x.SectionalArea).Distinct().ToList();
                    ViewBag.weldGeometries = statistics.Select(x => x.WeldType).Distinct().ToList();
                    ViewBag.weldLocations = statistics.Select(x => x.WeldLocationType).Distinct().ToList();
                    ViewBag.WeldingModels = statistics.Select(x => x.WeldingModel).Distinct().ToList();
                    ViewBag.ThickTypes = statistics.Select(x => x.ThickType).Distinct().ToList(); 
                    return View(model);
                }
                else
                {
                    return PartialView("InnerError", "无法找到焊缝记录，请在焊缝列表中寻找对应焊缝内容并修改。");
                }
            }
            return PartialView("InnerError","无法找到焊缝记录，请在焊缝列表中寻找对应焊缝内容并修改。");
        }

        [HttpPost]
        [ValidateInput(true)]
        public override JsonResult Post(WeldCategoryLabelingView objModal, string Id)
        {
            if (string.IsNullOrEmpty(objModal.Ids) || !objModal.Ids.Contains(","))
            {
                return base.Post(objModal, Id);
            }
            //批量更新
            if (!ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            var ids = objModal.Ids.TrimEnd(',').Split(',');
            foreach (var itemId in ids)
            {
                var intId = int.Parse(itemId);
                var item = _BaseService.GetList<WeldCategoryLabelingView>(1, x => x.Id == intId).FirstOrDefault();
                if (item != null && item.Id > 0)
                {
                    item.FigureNumber = objModal.FigureNumber;
                    item.BoardNumber = objModal.BoardNumber;
                    item.Thickness = objModal.Thickness;
                    item.WeldType = objModal.WeldType;
                    item.Thickness = objModal.Thickness;
                    item.WeldLocation = objModal.WeldLocation;
                    item.ConsumeFactor = objModal.ConsumeFactor;
                    item.MentalDensity = objModal.MentalDensity;
                    item.SectionArea = objModal.SectionArea;
                    item.WeldLength = objModal.WeldLength;
                    item.WeldQuanlity = objModal.WeldQuanlity;
                    item.WeldingNumber = objModal.WeldingNumber;
                    item.WeldingQuanlity = objModal.WeldingQuanlity;
                    item.BeamNum = objModal.BeamNum;
                    item.Quantity = objModal.Quantity;
                    item.LengthVal = objModal.LengthVal;
                    item.WidthVal = objModal.WidthVal;

                    _BaseService.UpdateView(item);
                }
            }            
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public ActionResult cadwelding()
        {
            return View();
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            string locatedId = Request["locatedId"];
            int beamId = 0;            
            Expression<Func<WeldCategoryLabeling, bool>> expression = FilterHelper.GetExpression<WeldCategoryLabeling>(gridRequest.FilterGroup);
            
            if (int.TryParse(Request["beamId"], out beamId))
            {
                expression = expression.AndAlso<WeldCategoryLabeling>(x => x.IsDeleted != true && x.BeamId == beamId);
            }
            else
            {
                expression = expression.AndAlso<WeldCategoryLabeling>(x => x.IsDeleted != true);
            }
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<WeldCategoryLabeling>(x => x.FigureNumber.Contains(strCondition));
            }
            if (!string.IsNullOrEmpty(locatedId))
            {
                int id = 0;
                int.TryParse(locatedId, out id);
                if (id > 0)
                {
                    expression = expression.AndAlso<WeldCategoryLabeling>(x => x.Id == id);
                }
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<WeldCategoryLabelingView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }

        public JsonResult GetWeldingByHandle(int beamId, string handleId)
        {
            var item = _weldCategoryService.GetList<WeldCategoryLabelingView>(1, x => !x.IsDeleted && x.BeamId == beamId && x.HandleID.Contains(handleId)).FirstOrDefault();
            if (item != null) {
                return new JsonResult { Data = new { result = "success", data = item.HandleID, item.Id }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult { Data = new { result = "failed" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetWeldingProperty(string figureNumber, string boardNumber)
        {
            var item = _boardPackageService.Repository.Entities.Where(x => !x.IsDeleted && x.FigureNumber == figureNumber && x.BoardNumber== boardNumber).FirstOrDefault();
            if (item != null)
            {
                return new JsonResult { Data = new { result = "success", data1 = item}, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult { Data = new { result = "failed" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult PostWeld(int? beamId, string dwgfile, List<WeldCategoryLabelingView> weldList)
        {
            if (string.IsNullOrEmpty(dwgfile) || !beamId.HasValue || weldList == null || weldList.Count == 0)
            {
                return new JsonResult { Data = new { result = "failed" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            var beam = _beamInfoService.GetList<BeamInfoView>(1, x => x.Id == beamId && !x.IsDeleted).FirstOrDefault();
            if(beam == null)
            {
                return new JsonResult { Data = new { result = "failed" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            var existing = _weldCategoryService.GetList<WeldCategoryLabelingView>(int.MaxValue, x => x.BeamId == beam.Id).ToList();

            foreach (var weldInfo in weldList)
            {
                if (!string.IsNullOrEmpty( weldInfo.HandleID) && !string.IsNullOrEmpty(weldInfo.WeldType))
                {
                    var existedItem = existing.FirstOrDefault(x => x.BeamId == beamId && x.HandleID.Contains(weldInfo.HandleID));
                    if (existedItem != null)
                    {
                        if (existedItem.WeldType != weldInfo.WeldType)
                        {
                            existedItem.WeldType = GetWeldType(weldInfo.WeldType);
                            weldInfo.Id = _weldCategoryService.UpdateView(existedItem);
                        }
                    }
                    else
                    {
                        if (weldInfo.CopyOriginId.HasValue && weldInfo.CopyOriginId > 0) //用于复制焊缝
                        {
                            var entity = _weldCategoryService.GetList<WeldCategoryLabelingView>(int.MaxValue, x => x.BeamId == beam.Id && x.Id == weldInfo.CopyOriginId).FirstOrDefault();

                            weldInfo.FigureNumber = entity.FigureNumber;
                            weldInfo.BoardNumber = entity.BoardNumber;
                            weldInfo.Thickness = entity.Thickness;
                            weldInfo.WeldType = entity.WeldType;
                            weldInfo.Thickness = entity.Thickness;
                            weldInfo.WeldLocation = entity.WeldLocation;
                            weldInfo.ConsumeFactor = entity.ConsumeFactor;
                            weldInfo.MentalDensity = entity.MentalDensity;
                            weldInfo.SectionArea = entity.SectionArea;
                            weldInfo.WeldLength = entity.WeldLength;
                            weldInfo.WeldQuanlity = entity.WeldQuanlity;
                            weldInfo.WeldingNumber = entity.WeldingNumber;
                            weldInfo.BeamNum = entity.BeamNum;
                            weldInfo.Quantity = entity.Quantity;
                            weldInfo.LengthVal = entity.LengthVal;
                            weldInfo.WidthVal = entity.WidthVal;
                            weldInfo.BeamId = beam.Id;
                            weldInfo.WeldType = entity.WeldType;
                        }
                        else
                        {
                            weldInfo.BeamId = beam.Id;
                            weldInfo.WeldType = GetWeldType(weldInfo.WeldType);
                        }
                        weldInfo.Id = _weldCategoryService.InsertView(weldInfo);
                    }
                }
            }
            if (beam.ProcessStatus == 0)
            {
                beam.ProcessStatus = (int)BeamProcessStatus.InProcessing;
                _beamInfoService.UpdateView(beam);
            }
            return new JsonResult { Data = new { result = "success", data = weldList.Count > 0 ? weldList[0].Id : 0 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult RequestApproval(int beamId)
        {
            if (beamId > 0)
            {
                var beam = _beamInfoService.GetList<BeamInfoView>(1, x => x.Id == beamId).FirstOrDefault();
                if (beam != null)
                {
                    var task = _taskListService.GetList<TaskListView>(1, x => !x.IsDeleted && x.ProjectId == beam.ProjectId && x.BeamId == beam.Id).FirstOrDefault();
                    if (task != null && (task.TaskStatus == (int)TaskStatus.PendingApproval || task.TaskStatus == (int)TaskStatus.Approved))
                    {
                        return new JsonResult { Data = new { result = "failed", msg = "本文件已提交，请勿重复提交" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    else if (task == null)
                    {
                        task = new TaskListView
                        {
                            ProjectId = beam.ProjectId,
                            BeamId = beam.Id,
                            DWGFile = beam.DwgFile
                        };
                    }
                    task.TaskStatus = (int)TaskStatus.PendingApproval;
                    if (task.Id == 0)
                    {
                        _taskListService.InsertView(task);
                    }
                    else
                    {
                        _taskListService.UpdateView(task);
                    }

                    if (beam.ProcessStatus != (int)BeamProcessStatus.Complete)
                    {
                        beam.ProcessStatus = (int)BeamProcessStatus.Complete;
                        _beamInfoService.UpdateView(beam);
                    }
                }
                return new JsonResult { Data = new { result = "success" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult { Data = new { result = "failed", msg="提交失败" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        private string GetFilePath(int projectId, string dwgfile)
        {
            var path = baseUrl + "Files/BeamInfo/" + projectId + "/" + dwgfile;
            if (!System.IO.File.Exists(Server.MapPath("~/Files/BeamInfo/" + projectId + "/" + dwgfile)))
            {
                path = baseUrl + "Files/BeamInfo/" + dwgfile;
            }
            return path;
        }
        private string GetFileAbsolutePath(int projectId, string dwgfile)
        {
            var path = "~/Files/BeamInfo/" + projectId + "/" + dwgfile;
            if (!System.IO.File.Exists(Server.MapPath("~/Files/BeamInfo/" + projectId + "/" + dwgfile)))
            {
                path = "~/Files/BeamInfo/" + dwgfile;
            }
            return Server.MapPath(path);
        }
        //TODO 需要更改为按配置识别
        private string GetWeldType(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return "";
            }
            switch (type.Trim())
            {
                case "N_PoKDuiJieH":
                    return "1-不开坡口对接焊";
                case "Y_DMDCPoKDuiJieH":
                    return "2-单面-单侧-坡口-背面封底-对接焊缝";
                case "Y_DMSCPoKDuiJieH":
                    return "3-双面-单侧-坡口-对接焊缝";
                case "Y_SMDCPoKDuiJieH":
                    return "4-双面单侧坡口";
                case "Y_SMSCPoKDuiJieH":
                    return "5-双面-双侧-坡口-对接焊缝";
                case "Y_DMDCPoKJiaoH":
                    return "6-单面-单侧-坡口-角焊缝";
                case "Y_DMPoKGaiBJiaoH":
                    return "7-单面-坡口-盖板-角焊缝";
                case "Y_SMPoKBeiPJiaoH":
                    return "8-双面-坡口-背面坡口-角焊缝";
                case "Y_SMPoKRTH":
                    return "10-双面-坡口-熔透";
                case "Y_SMPoKGaiBRTH":
                    return "11-双面坡口盖板熔透,有图形";
                case "Y_SMDuiJieH":
                    return "12-双面角焊缝";
                case "Y_DMJiaoH":
                    return "13-单面-角焊缝";
                case "Y_SMJiaoH":
                    return "14-双面角焊缝";
                case "ManJiaoH":
                    return "角焊缝";
                case "ManDuiJieH":
                    return "对接焊缝";
                case "ManPoKouH":
                    return "坡口焊缝";
                case "ManRongTouH":
                    return "熔透焊缝";
                default:
                    return "其他";
            }
        }
    }
}