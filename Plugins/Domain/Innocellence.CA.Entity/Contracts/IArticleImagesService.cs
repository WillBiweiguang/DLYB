using Infrastructure.Core;
using DLYB.CA.Entity;
using System.Collections.Generic;

namespace DLYB.CA.Contracts
{
    public interface IArticleImagesService : IDependency, IBaseService<ArticleImages>
    {
        List<T> GetListByAppID<T>(int appId) where T : IViewModel, new();
    }
}
