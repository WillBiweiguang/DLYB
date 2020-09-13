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
    public class WeldGeometryController : BaseController<WeldGeometry, WeldGeometryView>
    {
        private readonly IWeldGeometryService _service;
        public WeldGeometryController(IWeldGeometryService service) : base(service)
        {
            _service = service;            
        }
        // GET: Address
        public override ActionResult Index()
        {
            //var list = _addressService.GetList<AddressView>(int.MaxValue, x => !x.IsDeleted).ToList();
                 
            return View();
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            Expression<Func<WeldGeometry, bool>> expression = FilterHelper.GetExpression<WeldGeometry>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<WeldGeometry>(x => x.WeldType.Contains(strCondition)&& x.IsDeleted != true);
            }
            else
            {
                expression = expression.AndAlso<WeldGeometry>(x => x.IsDeleted != true);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<WeldGeometryView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }

        public JsonResult GetDropdownList(string keyword = "")
        {
            var list = _service.GetList<WeldGeometryView>(int.MaxValue, x => !x.IsDeleted && x.WeldType.Contains(keyword.Trim()))
                        .Select(x => new { key = x.Id, value = x.WeldType }).ToList();
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}