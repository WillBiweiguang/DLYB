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
using Infrastructure.Web.Domain.Common;

namespace DLYB.Web.Controllers
{
    public class CategoryStatController : BaseController<WeldCategoryStatisticsV, WeldCategoryStatisticsViewModel>
    {
        private readonly IWeldCategoryStatisticsService _service;
        private readonly IWeldCategoryStatisticsVService _wcsvService;
        private readonly IGrooveTypeService _GrooveTypeService;
        private readonly IBeamInfoService _beamInfoService;
        private readonly IProjectService _projectService;
        public CategoryStatController(IWeldCategoryStatisticsService service,
            IWeldCategoryStatisticsVService wcsvService,
            IGrooveTypeService grooveTypeService, IBeamInfoService beamInfoService,
            IProjectService projectService) : base(wcsvService)
        {
            _service = service;
            _wcsvService = wcsvService;
            _GrooveTypeService = grooveTypeService;
            _beamInfoService = beamInfoService;
            _projectService = projectService;                
        }
        // GET: Address
        public override ActionResult Index()
        {
            ViewBag.GrooveTypes = _GrooveTypeService.GetGrooveTypeQuerys();
            ViewBag.BeamId = Request["beamId"];
            ViewBag.ProjectId = Request["ProjectId"];
            ViewBag.IsAdmin = this.objLoginInfo.Menus.Any(x => x.Id == (int)EnumMenuId.WeldingMetaManage);
            int pid = 0;
            if (int.TryParse(Request["ProjectId"], out pid)){
                var project = _projectService.Repository.Entities.FirstOrDefault(x => x.Id == pid);
                ViewBag.ProjectName = project.ProjectName;
            }
            int bid = 0;
            if (int.TryParse(Request["beamId"], out bid))
            {
                var beam = _beamInfoService.Repository.Entities.FirstOrDefault(x => x.Id == bid);
                if (beam.DwgFile.IndexOf("dwg")>0)
                {
                    ViewBag.BeamName = beam.DwgFile.Substring(0, beam.DwgFile.IndexOf("dwg") - 1);
                }
                else if(beam.DwgFile.IndexOf("dxf") > 0)
                {
                    ViewBag.BeamName = beam.DwgFile.Substring(0, beam.DwgFile.IndexOf("dxf") - 1);
                }
               
            }
            ViewBag.ForthNav = "焊缝类别统计";
            return View();
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            int beamId = 0;
            int.TryParse(Request["beamId"] ?? "", out beamId);
            Expression<Func<WeldCategoryStatisticsV, bool>> expression = FilterHelper.GetExpression<WeldCategoryStatisticsV>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<WeldCategoryStatisticsV>(x => x.AddressName.Contains(strCondition) && x.IsDeleted != true);
            }
            else
            {
                expression = expression.AndAlso<WeldCategoryStatisticsV>(x => !x.IsDeleted);
            }
            if(beamId > 0)
            {
                expression = expression.AndAlso<WeldCategoryStatisticsV>(x => x.BeamId == beamId);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<WeldCategoryStatisticsViewModel> listEx = GetListEx(expression, gridRequest.PageCondition);
            var projectIDs = listEx.Select(x => x.ProjectId).ToList();
            var projects = _projectService.GetList<ProjectView>(int.MaxValue, x => projectIDs.Contains(x.Id)).ToList();
            listEx.ForEach(w => {
                var p = projects.FirstOrDefault(x => x.Id == w.ProjectId);
                if (p != null)
                {
                    w.ProjectName = p.ProjectName;
                }
            });
            return this.GetPageResult(listEx, gridRequest);
        }

        public override ActionResult Get(string id)
        {
            int intId = int.Parse(id);
            var entity = _service.GetList<WeldCategoryStatisticsView>(1, x => !x.IsDeleted && x.Id == intId).FirstOrDefault();
            var view = _wcsvService.GetList<WeldCategoryStatisticsVView>(1, x => !x.IsDeleted && x.Id == intId).FirstOrDefault();
            if (entity == null || view == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            else
            {
                WeldCategoryStatisticsViewModel viewModel = new WeldCategoryStatisticsViewModel
                {
                    Id = entity.Id,
                    ProjectId = entity.ProjectId,
                    AddressId = entity.AddressId,
                    HanjieId = entity.HanjieId,
                    WeldLocationId = entity.WeldLocationId,
                    WeldTypeId = entity.WeldTypeId,
                    ThicknessId = entity.ThicknessId,
                    GrooveTypeId = entity.GrooveTypeId,
                    SectionalArea = entity.SectionalArea,
                    CreatedDate = entity.CreatedDate,
                    CreatedUserID = entity.CreatedUserID,
                    UpdatedDate = entity.UpdatedDate,
                    UpdatedUserID = entity.UpdatedUserID,
                    IsDeleted = entity.IsDeleted,
                    ProjectName = view.ProjectName,
                    AddressName = view.AddressName,
                    HanJieType = view.HanJieType,
                    WeldLocationType = view.WeldLocationType,
                    WeldType = view.WeldType,
                    ThickType = view.ThickType,
                    GrooveType = view.GrooveType,
                    WeldingId = entity.WeldingId,
                    WeldingModel = view.WeldingModel
                };
                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(true)]
        public override JsonResult Post(WeldCategoryStatisticsViewModel objModal, string Id)
        {
            //验证错误
            if (!ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            var entity = objModal.ConvertView();
            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                _service.InsertView(entity);
            }
            else
            {
                _service.UpdateView(entity);
            }
            //更新文件状态
            var beam = _beamInfoService.GetList<BeamInfoView>(1, x => !x.IsDeleted && x.Id == entity.BeamId).FirstOrDefault();
            if (beam != null && beam.ProcessStatus == 0)
            {
                beam.ProcessStatus = 1;
                _beamInfoService.UpdateView(beam);
            }
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public override JsonResult Delete(string sIds)
        {
            if (!string.IsNullOrEmpty(sIds))
            {
                int[] arrID = sIds.TrimEnd(',').Split(',').Select(a => int.Parse(a)).ToArray();
                _service.Repository.Delete(arrID);
                AfterDelete(sIds);
            }
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }
    }
}