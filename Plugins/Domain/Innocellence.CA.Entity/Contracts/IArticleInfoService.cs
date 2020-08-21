using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using DLYB.CA.Entity;
using System.Linq.Expressions;

namespace DLYB.CA.Contracts
{
    public interface IArticleInfoService : IDependency, IBaseService<ArticleInfo>
    {
        List<T> GetListByCode<T>(string Newscode) where T : IViewModel, new();
        ArticleInfo GetByCateSub(string articleCateSub);
        T GetById<T>(int id) where T : IViewModel, new();

        List<T> GetList<T>(Expression<Func<ArticleInfo, bool>> predicate) where T : IViewModel, new();

        int UpdateArticleThumbsUp(int articleId, string userId, string type);

        string EncryptorLillyid(string lillyId);

        string DecryptLillyid(string encryLillyId);
        List<T> GetArticleList<T>(Expression<Func<ArticleInfo, bool>> predicate) where T : IViewModel, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="expirationTime">default is 1 m</param>
        /// <returns></returns>
        ArticleInfo GetArticleFromCache(int id, int? expirationTime = null);

        void BackendUpdateReadCount(int id);
    }
}
