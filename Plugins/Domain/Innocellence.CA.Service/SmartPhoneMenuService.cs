using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using Infrastructure.Core.Data;
using Infrastructure.Utility;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Extensions;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Service;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.ViewModel;
using DLYB.CA.Service.Common;
using DLYB.Weixin.Entities;
using DLYB.Weixin.QY;
using DLYB.Weixin.QY.AdvancedAPIs.MailList;
using DLYB.Weixin.QY.CommonAPIs;

namespace DLYB.CA.Service
{
    public class SmartPhoneMenuService : BaseService<Category>, ISmartPhoneMenuService
    {
        private const string _endFix = "ENTRY_PUSH";

        public QyJsonResult Push(int appId)
        {
            var categories = CommonService.lstCategory.Where(x => x.AppId == appId && (x.CategoryCode == null || !x.CategoryCode.EndsWith(_endFix, true, CultureInfo.CurrentCulture)) && (x.IsDeleted == null || x.IsDeleted == false) && (x.IsVirtual == null || x.IsVirtual == false)).OrderBy(x => x.CategoryOrder).Select(x =>
            {
                var function = x.Function.IsNullOrEmpty() ? null : JsonHelper.FromJson<ButtonReturnType>(x.Function);
                return new CategroyChild
                      {
                          Id = x.Id,
                          key = x.CategoryCode,
                          name = function == null ? x.CategoryName : function.Button.name,
                          sub_button = new List<MenuFull_RootButton>(),
                          type = function == null ? null : function.Button.type,
                          ParentId = x.ParentCode.GetValueOrDefault(),
                          order = x.CategoryOrder.GetValueOrDefault(),
                          url = function == null ? null : function.Button.url,
                      };
            }).ToList();

            var buttons = new List<MenuFull_RootButton>();

            categories.Where(x => x.ParentId == 0).ToList().ForEach(x => GenerateMenuHierarchy(categories, x, buttons));

            var menu = new GetMenuResultFull { menu = new MenuFull_ButtonGroup { button = buttons } };
            var btnMenus = CommonApi.GetMenuFromJsonResult(menu).menu;
            var result = CommonApi.CreateMenu(WeChatCommonService.GetWeiXinToken(appId), appId, btnMenus);
            return result;
        }

        private void GenerateMenuHierarchy(IEnumerable<CategroyChild> list, CategroyChild parent, IList<MenuFull_RootButton> buttons)
        {
            var categroyChildren = list as IList<CategroyChild> ?? list.ToList();
            var currentLayerChilds = categroyChildren.Where(x => x.ParentId == parent.Id).ToList();

            parent.sub_button.AddRange(currentLayerChilds.OrderBy(x => x.order).ToList());

            if (parent.ParentId == 0)
            {
                buttons.Add(parent);
            }

            currentLayerChilds.ForEach(x => GenerateMenuHierarchy(categroyChildren, x, buttons));
        }

        public AppMenuView QueryMenuViewById(int menuId)
        {
            var category = CommonService.lstCategory.FirstOrDefault(x => x.Id == menuId) ?? Repository.GetByKey(menuId);
            if (category == null)
            {
                return null;
            }

            category.IsAdmin = category.IsAdmin.GetValueOrDefault();
            var view = (AppMenuView)(new AppMenuView().ConvertAPIModel(category));
            view.TagItems = WeChatCommonService.lstTag;
            view.SelecTagItems = category.Role.IsNullOrEmpty() ? null : category.Role.Split(',').ToList().ConvertAll(x => new TagItem { tagname = x });
            view.ButtonReturnType = category.Function.IsNullOrEmpty() ? null : JsonHelper.FromJson<ButtonReturnType>(category.Function);

            return view;
        }

        public int UpdateOrAdd(AppMenuView menu)
        {
            menu.Function = menu.ButtonReturnType == null ? null : JsonHelper.ToJson(menu.ButtonReturnType);
            //menu.Role = menu.SelecTagItems == null || menu.SelecTagItems.Count == 0
            //    ? null
            //    : menu.SelecTagItems.Select(x => x.tagname).Join(",");

            var count = 0;

            if (!menu.ParentCode.HasValue)
            {
                menu.ParentCode = 0;
            }

            if (menu.Id == 0)
            {
                count = base.InsertView((CategoryView)menu);
            }
            else
            {
                //删除菜单
                if (menu.IsDeleted.HasValue && menu.IsDeleted.Value)
                {
                    var menuGroup =
                           CommonService.lstCategory.Where(x => x.ParentCode == menu.Id || x.Id == menu.Id).ToList();


                    using (
                        var transactionscope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                    {
                        menuGroup.ForEach(x =>
                                             {
                                                 x.IsDeleted = true;
                                                 Repository.Update(x);
                                             });
                        transactionscope.Complete();
                    }
                }
                else
                {
                    count = base.UpdateView((CategoryView)menu, new List<string>
                    {
                        "CategoryCode","AppId","CategoryName","CategoryDesc",
                        "ParentCode","IsAdmin","IsVirtual","CategoryOrder","Function","NoRoleMessage","Role"
                    });
                }
            }

            CommonService.RemoveCategoryFromCache();

            return count;
        }
    }

    public class CategroyChild : MenuFull_RootButton
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public int order { get; set; }
    }
}
