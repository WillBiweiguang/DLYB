using Infrastructure.Core;
using Infrastructure.Core.Data;
using DLYB.CA.Contracts;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace DLYB.CA.Services
{
    /// <summary>
    /// 业务实现——新闻
    /// </summary>
    public partial class ArticleImagesService : BaseService<ArticleImages>, IArticleImagesService
    {
        public ArticleImagesService()
            : base("CAAdmin")
        {
            
        }

        public List<T> GetListByAppID<T>(int appId) where T : IViewModel, new()
        {
            Expression<Func<ArticleImages, bool>> predicate = a => a.AppId == appId;
            var ens = Repository.Entities.ToList();

            var lst = ens.Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
        }

    }
}