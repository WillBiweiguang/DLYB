using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Service;
using DLYB.CA.Contracts;
using DLYB.CA.Entity;
using DLYB.CA.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Service
{
    public class SearchKeywordService : BaseService<SearchKeyword>, ISearchKeywordService
    {
        public SearchKeywordService(IUnitOfWork unitOfWork)
            : base("CAAdmin")
        {
        }

       public List<string> GetSearchKeywordsByCategory()
        {
            //var lst = Repository.Entities.Where(a => a.AppId == appId);
            //if(topNum != -1)
            //{
            //    lst = lst.OrderByDescending(a=>a.SearchCount).Take(topNum);
            //}

            //return lst.ToList().Select(n => (SearchKeywordView)new SearchKeywordView().ConvertAPIModel(n)).ToList();
            var jsonString = CommonService.GetSysConfig("FAQSearchKeyWord", "");
           var lst=new List<string>();
           foreach (var s in jsonString.Split(','))
           {
               lst.Add(s);
           }
           return lst;

        }

    }
}
