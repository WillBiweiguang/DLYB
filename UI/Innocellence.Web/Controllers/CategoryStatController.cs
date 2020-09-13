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

namespace DLYB.Web.Controllers
{
    public class CategoryStatController : BaseController<WeldCategoryStatisticsV, WeldCategoryStatisticsViewModel>
    {
        private readonly IWeldCategoryStatisticsService _service;
        private readonly IWeldCategoryStatisticsVService _wcsvService;
        public CategoryStatController(IWeldCategoryStatisticsService service,
            IWeldCategoryStatisticsVService wcsvService) : base(wcsvService)
        {
            _service = service;
            _wcsvService = wcsvService;
        }
        // GET: Address
        public override ActionResult Index()
        {                 
            return View();
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            Expression<Func<WeldCategoryStatisticsV, bool>> expression = FilterHelper.GetExpression<WeldCategoryStatisticsV>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<WeldCategoryStatisticsV>(x => x.AddressName.Contains(strCondition) && x.IsDeleted != true);
            }
            else
            {
                expression = expression.AndAlso<WeldCategoryStatisticsV>(x => !x.IsDeleted);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<WeldCategoryStatisticsViewModel> listEx = GetListEx(expression, gridRequest.PageCondition);
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