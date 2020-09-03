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
    public class AddressController : BaseController<Address,AddressView>
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService) : base(addressService)
        {
            _addressService = addressService;            
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
            Expression<Func<Address, bool>> expression = FilterHelper.GetExpression<Address>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<Address>(x => x.AddressName.Contains(strCondition));
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<AddressView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }     
    }
}