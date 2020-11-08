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
    public class GrooveTypeTextController : BaseController<GrooveTypesText, GrooveTypeTextView>
    {
        private readonly IGrooveTypeTextService _GrooveTypeService;

        public GrooveTypeTextController(IGrooveTypeTextService grooveTypeService) : base(grooveTypeService)
        {
            _GrooveTypeService = grooveTypeService;
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
            Expression<Func<GrooveTypesText, bool>> expression = FilterHelper.GetExpression<GrooveTypesText>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<GrooveTypesText>(x => x.GrooveType.Contains(strCondition) && x.IsDeleted != true);
            }
            else
            {
                expression = expression.AndAlso<GrooveTypesText>(x => x.IsDeleted != true);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<GrooveTypeTextView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }

       
    }
}