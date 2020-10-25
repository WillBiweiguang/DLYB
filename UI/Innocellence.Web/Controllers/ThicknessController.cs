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
    public class ThicknessController : BaseController<Thickness,ThicknessView>
    {
        private readonly IThicknessService _service;
        private readonly ISysUserService _sysUserService;
        public ThicknessController(IThicknessService service, ISysUserService sysUserService) : base(service)
        {
            _service = service;
            _sysUserService = sysUserService;
        }
        
        public override ActionResult Index()
        {
            return View();
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            Expression<Func<Thickness, bool>> expression = FilterHelper.GetExpression<Thickness>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<Thickness>(x => x.ThickType.Contains(strCondition) && x.IsDeleted != true);
            }
            else
            {
                expression = expression.AndAlso<Thickness>(x => x.IsDeleted != true);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<ThicknessView> listEx = GetListEx(expression, gridRequest.PageCondition);
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
            var list = _service.GetList<ThicknessView>(int.MaxValue, x => !x.IsDeleted && x.ThickType.Contains(keyword.Trim()))
                        .Select(x => new { key = x.Id, value = x.ThickType }).ToList();
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}