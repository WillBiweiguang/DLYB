using DLYB.Web.Controllers;
using Infrastructure.Utility.Filter;
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

        private readonly string baseUrl = "http://42.202.130.245:3001/";
        public WeldCategoryController(IWeldCategoryLabelingService weldCategoryService,
            IBeamInfoService beamInfoService,IWeldGeometryService weldGeometryService,
            IWeldLocationService weldLocationService) : base(weldCategoryService)
        {
            _weldCategoryService = weldCategoryService;
            _beamInfoService = beamInfoService;
            _weldGeometryService = weldGeometryService;
            _weldLocationService = weldLocationService;
            baseUrl = ConfigurationManager.AppSettings["WebUrl"];
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
                    ViewBag.ProjectId = beam.ProjectId;
                    ViewBag.FileName = beam.DwgFile;
                    ViewBag.FilePath = GetFilePath(beam.ProjectId, beam.DwgFile);
                }
            }
            ViewBag.weldCategorys = _weldCategoryService.Repository.Entities.Where(a => !a.IsDeleted).ToList();
            ViewBag.weldGeometries = _weldGeometryService.Repository.Entities.Where(x => !x.IsDeleted).ToList();
            ViewBag.weldLocations = _weldLocationService.Repository.Entities.Where(x => !x.IsDeleted).ToList();
            return View();
        }

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

        public override ActionResult Edit(string Id)
        {
            WeldCategoryLabelingView model = new WeldCategoryLabelingView();
            if (!string.IsNullOrEmpty(Id))
            {
                model = GetObject(Id);
                var beam = _beamInfoService.GetList<BeamInfoView>(1, x => x.Id == model.BeamId).FirstOrDefault();
                if (beam != null)
                {
                    ViewBag.BeamId = beam.Id;
                    ViewBag.ProjectId = beam.ProjectId;
                    ViewBag.FileName = beam.DwgFile;
                }
                ViewBag.weldCategorys = _weldCategoryService.Repository.Entities.Where(a => !a.IsDeleted).ToList();
                ViewBag.weldGeometries = _weldGeometryService.Repository.Entities.Where(x => !x.IsDeleted).ToList();
                ViewBag.weldLocations = _weldLocationService.Repository.Entities.Where(x => !x.IsDeleted).ToList();
            }
            return View(model);
        }

        //public ActionResult Edit(string handleId, int beamId = 0)
        //{
        //    WeldCategoryLabelingView model = new WeldCategoryLabelingView
        //    {
        //        HandleID = handleId,
        //        BeamId = beamId
        //    };
        //    if (beamId > 0 && string.IsNullOrEmpty(handleId))
        //    {
        //        model = _weldCategoryService.GetList<WeldCategoryLabelingView>(1, x => !x.IsDeleted && x.BeamId == beamId && x.HandleID.Contains(handleId)).FirstOrDefault();
        //    }        
        //    return View(model);
        //}

        public ActionResult cadwelding()
        {
            return View();
        }


        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
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
            //if (existing.Count > 0)
            //{
            //    return new JsonResult { Data = new { result = "failed", message = "当前文件已识别，请不要重复识别" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //}
            foreach (var weldInfo in weldList)
            {
                if (!string.IsNullOrEmpty( weldInfo.HandleID) && !string.IsNullOrEmpty(weldInfo.WeldType))
                {
                    var existedItem = existing.FirstOrDefault(x => x.BeamId == weldInfo.BeamId && x.HandleID.Contains(weldInfo.HandleID));
                    if (existedItem != null)
                    {
                        if (existedItem.WeldType != weldInfo.WeldType)
                        {
                            existedItem.WeldType = weldInfo.WeldType;
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
                        }
                        weldInfo.BeamId = beam.Id;
                        weldInfo.WeldType = GetWeldType(weldInfo.WeldType);
                        weldInfo.Id = _weldCategoryService.InsertView(weldInfo);
                    }
                }
            }
            return new JsonResult { Data = new { result = "success", data = weldList.Count > 0 ? weldList[0].Id : 0 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        private string GetFilePath(int projectId, string dwgfile)
        {
            var path = baseUrl + "Files/BeamInfo/" + projectId + "/" + dwgfile;
            if (!System.IO.File.Exists(Server.MapPath("/Files/BeamInfo/" + projectId + "/" + dwgfile)))
            {
                path = baseUrl + "Files/BeamInfo/" + dwgfile;
            }
            return path;
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
                default:
                    return "其他";
            }
        }
    }
}