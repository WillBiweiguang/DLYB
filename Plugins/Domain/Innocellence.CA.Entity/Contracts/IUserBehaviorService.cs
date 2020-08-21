using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using DLYB.CA.Entity;
using DLYB.CA.ModelsView;
using DLYB.CA.Contracts.ViewModel;
using Infrastructure.Utility.Data;

namespace DLYB.CA.Contracts
{
    public interface IUserBehaviorService : IDependency, IBaseService<UserBehavior>
    {
        List<UserBehaviorView> GetByList(DateTime begindate, DateTime endTime);
        List<int> GetAgentList(DateTime begindate, DateTime endTime);
        List<UserBehaviorArticleReportView> GetArticleList(DateTime begindate, DateTime endTime, string appid, PageCondition ConPage);
        List<UserBehaviorArticleReportView> GetChanelList(DateTime begindate, DateTime endTime, string appId, PageCondition pageCondition);
        List<UserBehaviorArticleReportView> GetMenuList(DateTime begindate, DateTime endTime, string appid, PageCondition pageCondition);
        List<UserBehaviorArticleReportView> GetActivityList(DateTime begindate, DateTime endTime, PageCondition pageCondition);
        void getNewsList(UserBehaviorArticleReportView articleView);
        void getMessagesList(UserBehaviorArticleReportView messageView);
        void getLEDList(UserBehaviorArticleReportView ledView);
        void getEmplist(List<UserBehaviorArticleReportView> lists);


        void Insert(UserBehavior userBehavior);
    }
}
