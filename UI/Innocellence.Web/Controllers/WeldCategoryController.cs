using DLYB.Web.Controllers;
using Infrastructure.Utility.Filter;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Innocellence.FaultSearch.Controllers
{
    public class WeldCategoryController : BaseController<WeldCategoryLabeling, WeldCategoryLabelingView>
    {
        private readonly IWeldCategoryLabelingService _weldCategoryService;

        public WeldCategoryController(IWeldCategoryLabelingService weldCategoryService) : base(weldCategoryService)
        {
            _weldCategoryService = weldCategoryService;
        }
        public override ActionResult Index()
        {
            ViewBag.weldCategorys = _weldCategoryService.Repository.Entities.Where(a => !a.IsDeleted).ToList();
            return View();
        }


        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            Expression<Func<WeldCategoryLabeling, bool>> expression = FilterHelper.GetExpression<WeldCategoryLabeling>(gridRequest.FilterGroup);

            expression = expression.AndAlso<WeldCategoryLabeling>(x => x.IsDeleted != true);

            int rowCount = gridRequest.PageCondition.RowCount;
            List<WeldCategoryLabelingView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }


    }
}