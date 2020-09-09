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
using DLYB.Web.Controllers;
using Infrastructure.Web.Domain.Services;

namespace Innocellence.Web.Controllers
{
    public class BeamInfoController : BaseController<BeamInfo, BeamInfoView>
    {
        private readonly IBeamInfoService _beamInfoService;

        public BeamInfoController(IBeamInfoService beamInfoService) : base(beamInfoService)
        {
            _beamInfoService = beamInfoService;
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
            Expression<Func<BeamInfo, bool>> expression = FilterHelper.GetExpression<BeamInfo>(gridRequest.FilterGroup);

            expression = expression.AndAlso<BeamInfo>(x => x.IsDeleted != true);

            int rowCount = gridRequest.PageCondition.RowCount;
            List<BeamInfoView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }
    }
}