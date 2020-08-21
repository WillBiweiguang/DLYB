using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.Contracts
{
    public interface IArticleInfoService : IDependency, IBaseService<ArticleInfo>
    {
        List<T> GetListByCode<T>(string Newscode) where T : IViewModel, new();
    }
}
