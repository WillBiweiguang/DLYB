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
    public class CategoryStatController : BaseController<WeldCategoryStatisticsV, WeldCategoryStatisticsVView>
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
                expression = expression.AndAlso<WeldCategoryStatisticsV>(x => x.IsDeleted != true);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<WeldCategoryStatisticsVView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }
    }
}