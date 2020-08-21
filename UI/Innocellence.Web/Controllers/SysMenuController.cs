using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Net;

using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.UI;
using System.Web.Mvc;
using Infrastructure.Core;
using DLYB.Web.Service;
using DLYB.Web.Extensions;
using System.Web;

using Infrastructure.Utility;
using Infrastructure.Web.Domain.Model;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Utility.Filter;

namespace DLYB.Web.Controllers
{
    public class SysMenuController : BaseController<SysMenu, SysMenuView>
    {

        ISysRoleMenuService ServiceRoleMenu;
        public SysMenuController(ISysMenuService newsService, ISysRoleMenuService _ServiceRoleMenu)
            : base(newsService)
        {
            ServiceRoleMenu = _ServiceRoleMenu;
        }


        public override ActionResult Index()
        {

            ViewBag.CateType = Request["CateType"];
            ViewBag.Apps = CommonService.lstSysWeChatConfig.ToDictionary(x => x.WeixinAppId);
            return base.Index();
        }


        public ActionResult GetListTree()
        {


            GridRequest req = new GridRequest(Request);
            Expression<Func<SysMenu, bool>> predicate = FilterHelper.GetExpression<SysMenu>(req.FilterGroup);
            int iCount = req.PageCondition.RowCount;

            predicate = predicate.AndAlso(a => a.IsDeleted == false);

            //实现对用户和多条件的分页的查询，rows表示一共多少条，page表示当前第几页
            string strModeId = Request["SearchLoadModeId"];
            string strGroup = Request["SearchGroup"];

            req.PageCondition.PageSize = 1000;


            // ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

            var q = _objService.GetList<SysMenuView>(predicate, req.PageCondition);

            // var pluginDescriptors = _pluginFinder.GetRoleDescriptors(loadMode, 0, strGroup).ToList();

            // return q.ToList();

            var list = q.Select(a => new
            {
                MenuTitle = a.MenuTitle,
                id = a.Id,
                name = a.MenuName,
                MenuImg = a.MenuImg,
                _parentId = a.ParentID,
                NavigateUrl = a.NavigateUrl,
                MenuGroup = a.MenuGroup,
                sortcode=a.SortCode

            }).OrderBy(a => a.sortcode).ToList();

            return Json(new
            {
                total = list.Count,
                rows = list
            }, JsonRequestBehavior.AllowGet);

        }

        public override ActionResult getTreeData()
        {

            string strID = Request["ID"];

            Expression<Func<SysMenu, bool>> predicate = FilterHelper.GetExpression<SysMenu>();

            predicate = predicate.AndAlso(a => a.IsDeleted == false);
            var list = _objService.GetList<SysMenuView>(1000, predicate).OrderBy(a => a.SortCode).ToList();

            var listReturn = EasyUITreeData.GetTreeData(list, "Id", "MenuName", "ParentID");

            if (!string.IsNullOrEmpty(strID))
            {
                var lstMenu = ServiceRoleMenu.GetMenusByRoleID(int.Parse(strID));

                EasyUITreeData.SetChecked(lstMenu, listReturn);

            }

            return Json(listReturn, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetListJQTree()
        {

            GridRequest req = new GridRequest(Request);
            Expression<Func<SysMenu, bool>> predicate = FilterHelper.GetExpression<SysMenu>(req.FilterGroup);
            int iCount = req.PageCondition.RowCount;

            //实现对用户和多条件的分页的查询，rows表示一共多少条，page表示当前第几页
            string strModeId = Request["SearchLoadModeId"];
            string strGroup = Request["SearchGroup"];

            req.PageCondition.PageSize = 1000;


            // ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

            var q = _objService.GetList<SysMenuView>(predicate, req.PageCondition);

            // var pluginDescriptors = _pluginFinder.GetRoleDescriptors(loadMode, 0, strGroup).ToList();

            // return q.ToList();

            var list = q.Select(a => new { MenuTitle = a.MenuTitle, Id = a.Id, MenuName = a.MenuName, MenuImg = a.MenuImg, ParentID = a.ParentID });

            return Json(new
            {
                totalpages = "3",
                currpage = "1",
                totalrecords = "11",
                griddata = list
            }, JsonRequestBehavior.AllowGet);
        }

        public void GetJqGridData()
        {

        }

        //初始化list页面
        public override List<SysMenuView> GetListEx(Expression<Func<SysMenu, bool>> predicate, PageCondition ConPage)
        {
            string strModeId = Request["SearchLoadModeId"];
            string strGroup = Request["SearchGroup"];

            ConPage.PageSize = 1000;

            // ConPage.SortConditions.Add(new SortCondition("CreatedDate", System.ComponentModel.ListSortDirection.Descending));

            var q = _objService.GetList<SysMenuView>(predicate, ConPage).OrderBy(a=>a.SortCode);

            // var pluginDescriptors = _pluginFinder.GetRoleDescriptors(loadMode, 0, strGroup).ToList();

            return q.ToList();
        }


        public override bool BeforeAddOrUpdate(SysMenuView objModal, string Id)
        {
            Session.Clear();
            return true;
        }

        public override bool AfterDelete(string sIds)
        {
            Session.Clear();
            return true;
        }

    }
}
