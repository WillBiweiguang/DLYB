using System.Collections.Generic;
using Infrastructure.Core;
using Infrastructure.Utility.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.ModelsView;
using DLYB.CA.Entity;


namespace DLYB.CA.Contracts.Contracts
{
    public interface IArticleHistoryService : IBaseService<ArticleInfo>, IDependency
    {

        List<ArticleInfoView> GetArticleHistoryList(int AppId, PageCondition condition, out int totalRow);
        List<ArticleInfoView> getMenuHistoryList(string MenuKey, PageCondition condition, out int totalRow);
    }

  
   
}
