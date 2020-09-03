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
    public class WeldingController : BaseController<Welding,WeldingView>
    {
        private readonly IWeldingService _weldingService;
        public WeldingController(IWeldingService weldingService) : base(weldingService)
        {
            _weldingService = weldingService;
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
            Expression<Func<Welding, bool>> expression = FilterHelper.GetExpression<Welding>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<Welding>(x => x.WeldingType.Contains(strCondition));
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<WeldingView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }
    }
}