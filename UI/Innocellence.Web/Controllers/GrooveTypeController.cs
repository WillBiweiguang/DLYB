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

namespace Innocellence.Web.Controllers
{
    public class GrooveTypeController : BaseController<GrooveTypes, GrooveTypeView>
    {
        private readonly IGrooveTypeService _GrooveTypeService;

        public GrooveTypeController(IGrooveTypeService grooveTypeService) : base(grooveTypeService)
        {
            _GrooveTypeService = grooveTypeService;
        }
        // GET: Address
        public override ActionResult Index()
        {
            //var list = _addressService.GetList<AddressView>(int.MaxValue, x => !x.IsDeleted).ToList();
                 
            return View();
        }
        public ActionResult Calculate()
        {
            ViewBag.GrooveTypes = _GrooveTypeService.GetList<GrooveTypeView>(int.MaxValue, x => !x.IsDeleted).ToList();

            return View();
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            Expression<Func<GrooveTypes, bool>> expression = FilterHelper.GetExpression<GrooveTypes>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<GrooveTypes>(x => x.GrooveType.Contains(strCondition) && x.IsDeleted != true);
            }
            else
            {
                expression = expression.AndAlso<GrooveTypes>(x => x.IsDeleted != true);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<GrooveTypeView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }
    }
}