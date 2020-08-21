using System.Transactions;
using EntityFramework.Extensions;
using Infrastructure.Core;
using Infrastructure.Core.Contracts;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.Service;
using DLYB.CA.Contracts;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Contracts.ViewModel;
using DLYB.CA.Entity;
using DLYB.CA.Service.Common;
using DLYB.CA.Services;
using DLYB.Weixin.QY.AdvancedAPIs.MailList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DLYB.CA.Service
{
    public class UserBehaviorService : BaseService<UserBehavior>, IUserBehaviorService
    {
        private readonly IMessageService _messageService;
        private readonly IArticleInfoService _articleService = new ArticleInfoService();
        public UserBehaviorService(IUnitOfWork unitOfWork, IMessageService messageService)
            : base(unitOfWork)
        {
            _messageService = messageService;
        }

        public List<UserBehaviorView> GetByList(DateTime begindate, DateTime endTime)
        {

            Expression<Func<UserBehavior, bool>> predicate = n => n.CreatedTime >= begindate && n.CreatedTime <= endTime;
            var t = Repository.Entities.Where(predicate).GroupBy(a => a.AppId).Select(a => new UserBehaviorView() { AppId = a.Key, Count = a.Count() }).ToList();

            return t;
        }
        public List<UserBehaviorArticleReportView> GetArticleList(DateTime begindate, DateTime endTime, string appid, PageCondition pageCondition)
        {

            Expression<Func<UserBehavior, bool>> predicate;
            if (!string.IsNullOrEmpty(appid))
            {
                var appId = Int32.Parse(appid);
                predicate = n => n.CreatedTime >= begindate && n.CreatedTime <= endTime && n.AppId == appId && n.Content != null && n.Content != "" && (n.ContentType == 1 || n.ContentType == 2 || n.ContentType == 51 || n.ContentType == 53);

            }
            else
            {
                predicate = n => n.CreatedTime >= begindate && n.CreatedTime <= endTime && n.Content != null && n.Content != "" && (n.ContentType == 1 || n.ContentType == 2 || n.ContentType == 51 || n.ContentType == 53);

            }
            var result = (from item in Repository.Entities.Where(predicate).ToList()
                          orderby item.Id descending
                          select new UserBehaviorArticleReportView()
                          {
                              Id = item.Id,
                              AppId = item.AppId,
                              CreatedTime = item.CreatedTime.ToShortDateString(),
                              UserId = item.UserId,
                              ContentType = item.ContentType,
                              Content = item.Content
                          }).ToList();

            result.ForEach(x =>
            {
                //根据ContentType去相应的表去拿最终集合
                switch (x.ContentType)
                {
                    case 1:
                        this.getNewsList(x);
                        break;
                    case 2:
                        this.getMessagesList(x);
                        break;

                    case 51:
                    case 52:
                    case 53:
                        this.getLEDList(x);
                        break;
                }
            });
            var lastResult = (from item in result where !item.NeedDelete select item).ToList();
            pageCondition.RowCount = lastResult.Count;
            pageCondition.PageIndex = pageCondition.PageIndex == 0 ? 1 : pageCondition.PageIndex;
            lastResult = lastResult.Skip((pageCondition.PageIndex - 1) * pageCondition.PageSize).Take(pageCondition.PageSize).ToList();
            pageCondition.SortConditions.Add(new SortCondition("CreatedTime", System.ComponentModel.ListSortDirection.Descending));
            return lastResult;
        }

        public List<UserBehaviorArticleReportView> GetChanelList(DateTime begindate, DateTime endTime, string appid, PageCondition pageCondition)
        {
            Expression<Func<UserBehavior, bool>> predicate;
            if (!string.IsNullOrEmpty(appid))
            {
                var appId = Int32.Parse(appid);
                predicate = n => n.CreatedTime >= begindate && n.CreatedTime <= endTime && n.AppId == appId && n.ContentType == 10;

            }
            else
            {
                predicate = n => n.CreatedTime >= begindate && n.CreatedTime <= endTime && n.ContentType == 10;

            }

            var result = (from item in Repository.Entities.Where(predicate).ToList()
                          orderby item.Id descending
                          select new UserBehaviorArticleReportView()
                          {
                              Id = item.Id,
                              AppId = item.AppId,
                              CreatedTime = item.CreatedTime.ToShortDateString(),
                              UserId = item.UserId,
                              ContentType = item.ContentType
                          }).ToList();
            result.ForEach(x =>
            {

                var app = CommonService.lstSysWeChatConfig.FirstOrDefault(y => y.WeixinAppId == x.AppId.ToString());
                if (app != null)
                {
                    x.AppName = app.AppName;
                }

            });
            pageCondition.RowCount = result.Count;
            pageCondition.PageIndex = pageCondition.PageIndex == 0 ? 1 : pageCondition.PageIndex;
            result = result.Skip((pageCondition.PageIndex - 1) * pageCondition.PageSize).Take(pageCondition.PageSize).ToList();
            pageCondition.SortConditions.Add(new SortCondition("CreatedTime", System.ComponentModel.ListSortDirection.Descending));
            return result;
        }
        public List<UserBehaviorArticleReportView> GetMenuList(DateTime begindate, DateTime endTime, string appid, PageCondition pageCondition)
        {

            Expression<Func<UserBehavior, bool>> predicate;
            if (!string.IsNullOrEmpty(appid))
            {
                var appId = Int32.Parse(appid);
                predicate = n => n.CreatedTime >= begindate && n.CreatedTime <= endTime && n.AppId == appId && (n.ContentType == 4 || n.ContentType == 5 || n.ContentType == 6 || n.ContentType == 7);
            }
            else
            {
                predicate = n => n.CreatedTime >= begindate && n.CreatedTime <= endTime && (n.ContentType == 4 || n.ContentType == 5 || n.ContentType == 6 || n.ContentType == 7);
            }
            var result = (from item in Repository.Entities.Where(predicate).ToList()
                          orderby item.Id descending
                          select new UserBehaviorArticleReportView()
                          {
                              Id = item.Id,
                              AppId = item.AppId,
                              CreatedTime = item.CreatedTime.ToShortDateString(),
                              UserId = item.UserId,
                              ContentType = item.ContentType,
                              Content = item.Content,
                              FunctionId = item.FunctionId
                          }).ToList();

            result.ForEach(x =>
            {
                x.MenuKey = x.Content;
                if (x.ContentType == 4)
                {
                    x.MenuKey = x.FunctionId;
                }
                var category = CommonService.lstCategory.FirstOrDefault(a => a.CategoryCode == x.MenuKey);
                if (category != null)
                {
                    x.MenuName = category.CategoryName;

                }
                var app = CommonService.lstSysWeChatConfig.FirstOrDefault(y => y.WeixinAppId == x.AppId.ToString());
                if (app != null)
                {
                    x.AppName = app.AppName;
                }

            });
            pageCondition.RowCount = result.Count;
            pageCondition.PageIndex = pageCondition.PageIndex == 0 ? 1 : pageCondition.PageIndex;
            result = result.Skip((pageCondition.PageIndex - 1) * pageCondition.PageSize).Take(pageCondition.PageSize).ToList();
            pageCondition.SortConditions.Add(new SortCondition("CreatedTime", System.ComponentModel.ListSortDirection.Descending));
            return result;
        }
        public void getNewsList(UserBehaviorArticleReportView articleView)
        {
            var arinfo = _articleService.Repository.Entities.FirstOrDefault(x => x.Id.ToString() == articleView.Content);
            if (arinfo != null && arinfo.IsDeleted == true)
            {

                articleView.NeedDelete = true;
            }
            else if (arinfo != null)
            {
                articleView.MenuKey = arinfo.ArticleCateSub;
                articleView.ArticleTitle = arinfo.ArticleTitle;
                var app = CommonService.lstSysWeChatConfig.FirstOrDefault(y => y.WeixinAppId == arinfo.AppId.ToString());
                if (app != null)
                {
                    articleView.AppName = app.AppName;
                }
                var category = CommonService.lstCategory.FirstOrDefault(a => a.CategoryCode == arinfo.ArticleCateSub);
                if (category != null)
                {
                    articleView.MenuName = category.CategoryName;
                }
            }

        }

        public void getMessagesList(UserBehaviorArticleReportView messageView)
        {
            var messageinfo = _messageService.Repository.Entities.FirstOrDefault(x => x.Id.ToString() == messageView.Content);
            if (messageinfo != null && messageinfo.IsDeleted == true)
            {
                messageView.NeedDelete = true;
            }
            else if (messageinfo != null)
            {
                messageView.ArticleTitle = messageinfo.Title;
                var app = CommonService.lstSysWeChatConfig.FirstOrDefault(y => y.WeixinAppId == messageinfo.AppId.ToString());
                if (app != null)
                {
                    messageView.AppName = app.AppName;
                }
            }

        }
        public void getLEDList(UserBehaviorArticleReportView ledView)
        {
            var objContent = new LEDContent();
            objContent = JsonHelper.FromJson<LEDContent>(ledView.Content);
            ledView.ArticleTitle = objContent.ArticleTitle;
            ledView.MenuKey = objContent.MeunKey;
            var ledCategory = CommonService.lstCategory.FirstOrDefault(z => z.CategoryCode == objContent.MeunKey);
            if (ledCategory != null)
            {
                ledView.MenuName = ledCategory.CategoryName;
            }
            var app = CommonService.lstSysWeChatConfig.FirstOrDefault(y => y.WeixinAppId == ledView.AppId.ToString());
            if (app != null)
            {
                ledView.AppName = app.AppName;
            }

        }
        public void getEmplist(List<UserBehaviorArticleReportView> lists)
        {
            List<EmployeeInfoWithDept> empDetails = WeChatCommonService.lstUserWithDeptTag;
            lists.ForEach(item =>
            {
                var emp = empDetails.SingleOrDefault(a => a.userid.ToUpper().Equals(item.UserId.ToUpper()));
                if (emp != null)
                {
                    item.deptLvs = emp.deptLvs;
                    item.UserName = emp.name;
                    item.gender = emp.gender == "1" ? "男" : "女";
                    item.email = emp.email;
                    item.status = emp.status == 1 ? "已关注" : "未关注";
                }
            }
         );
        }

        public List<int> GetAgentList(DateTime begindate, DateTime endTime)
        {
            Expression<Func<UserBehavior, bool>> predicate = n => n.CreatedTime >= begindate && n.CreatedTime <= endTime;
            var t = Repository.Entities.Where(predicate).Select(a => a.AppId).Distinct().ToList();
            return t;
        }

        public List<UserBehaviorArticleReportView> GetActivityList(DateTime begindate, DateTime endTime, PageCondition pageCondition)
        {
            List<UserBehaviorArticleReportView> result;

            Expression<Func<UserBehavior, bool>> predicate = n => n.CreatedTime >= begindate && n.CreatedTime <= endTime;


            var source = Repository.Entities.Where(predicate).Select(x => new { x.UserId, x.ContentType }).Where(x => x.ContentType != 9 && !string.IsNullOrEmpty(x.UserId)).GroupBy(x => x.UserId).Select(x => new { x.Key, followCount = x.Count() });

            if (pageCondition != null)
            {
                var countQuery = source.FutureCount();
                var conditionQuery =
                    source.OrderByDescending(x => x.Key)
                        .Skip((pageCondition.PageIndex - 1) * pageCondition.PageSize)
                        .Take(pageCondition.PageSize)
               .Future();

                pageCondition.RowCount = countQuery.Value;
                return result =
                           conditionQuery.ToList().AsParallel()
                               .Select(x => new UserBehaviorArticleReportView { UserId = x.Key, followCount = x.followCount })
                               .ToList();
            }


            result = source.ToList().AsParallel().Select(x => new UserBehaviorArticleReportView { UserId = x.Key, followCount = x.followCount }).ToList();

            return result;
            //logs.GroupBy(x => x.UserId).ToList().ForEach(item =>
            //{
            //    var item1 = item.FirstOrDefault();
            //    var entity = new UserBehaviorArticleReportView
            //       {
            //           Id = item1.Id,
            //           UserId = item1.UserId,
            //           followCount = item.Count()
            //       };
            //    result.Add(entity);
            //});
            //pageCondition.RowCount = result.Count;
            //pageCondition.PageIndex = pageCondition.PageIndex == 0 ? 1 : pageCondition.PageIndex;
            //result = result.Skip((pageCondition.PageIndex - 1) * pageCondition.PageSize).Take(pageCondition.PageSize).ToList();
            //pageCondition.SortConditions.Add(new SortCondition("CreatedTime", System.ComponentModel.ListSortDirection.Descending));
            //return result;

        }

        public void Insert(UserBehavior userBehavior)
        {
            using (var trans = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                Repository.Insert(userBehavior);

                trans.Complete();
            }
        }
    }
}
