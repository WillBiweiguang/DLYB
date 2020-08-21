using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using DLYB.CA.Entity;
using DLYB.CA.ModelsView;
namespace DLYB.CA.Contracts
{
    public interface ISearchKeywordService : IDependency, IBaseService<SearchKeyword>
    {
        List<string> GetSearchKeywordsByCategory();
    }
}
