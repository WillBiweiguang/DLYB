﻿using System;
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
    public class HanJieLocationController : BaseController<HanJieLocation, HanJieLocationView>
    {
        private readonly IHanJieLocationService _service;
        private readonly ISysUserService _sysUserService;
        public HanJieLocationController(IHanJieLocationService service,ISysUserService userService) : base(service)
        {
            _service = service;
            _sysUserService = userService;
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
            Expression<Func<HanJieLocation, bool>> expression = FilterHelper.GetExpression<HanJieLocation>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<HanJieLocation>(x => x.HanJieType.Contains(strCondition) && x.IsDeleted != true);
            }
            else
            {
                expression = expression.AndAlso<HanJieLocation>(x => x.IsDeleted != true);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<HanJieLocationView> listEx = GetListEx(expression, gridRequest.PageCondition);
            var userIds = listEx.Select(x => x.CreatedUserID).ToArray();
            var users = _sysUserService.GetList<SysUserView>(10, x => userIds.Contains(x.Id)).ToList();
            listEx.ForEach(x =>
            {
                x.CreatedUserName = users.FirstOrDefault(e => e.Id == x.UpdatedUserID)?.UserTrueName;
            });
            return this.GetPageResult(listEx, gridRequest);
        }

        public JsonResult GetDropdownList(string keyword = "")
        {
            var list = _service.GetList<HanJieLocationView>(int.MaxValue, x => !x.IsDeleted && x.HanJieType.Contains(keyword.Trim()))
                        .Select(x => new { key = x.Id, value = x.HanJieType }).ToList();
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}