// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 17:21</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Infrastructure.Core;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Data;
using Infrastructure.Core.Infrastructure;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Entity;
using DLYB.CA.Contracts;
using System.Linq.Expressions;
using DLYB.CA.Service;
using DLYB.CA.Service.Common;
using DLYB.CA.ModelsView;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.Domain.Service.Common;


namespace DLYB.CA.Services
{
    /// <summary>
    /// 业务实现——新闻
    /// </summary>
    public partial class ArticleInfoService : BaseService<ArticleInfo>, IArticleInfoService
    {
        private IImageInfoService _imageService;
        private const string keyName = "UrlEncryptionKey";

        private readonly ICacheManager _cacheManager;
        private const string CacheKeyPrefix = "article_cache_{0}";

        private readonly IArticleThumbsUpService _articleThumbsUpService;
        public ArticleInfoService(IUnitOfWork unitOfWork,
            IImageInfoService imageService,
            IArticleThumbsUpService articleThumbsUpService, ICacheManager cacheManager)
            : base("CAAdmin")
        {
            _imageService = imageService;
            _articleThumbsUpService = articleThumbsUpService;
            _cacheManager = cacheManager;
        }

        public ArticleInfoService()
            : base("CAAdmin")
        {

        }

        public T GetById<T>(int id) where T : IViewModel, new()
        {
            Expression<Func<ArticleInfo, bool>> predicate = a => a.Id == id && a.IsDeleted == false;

            var t = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).FirstOrDefault();

            return t;
        }

        public ArticleInfo GetByCateSub(string articleCateSub)
        {
            Expression<Func<ArticleInfo, bool>> predicate = a => a.IsDeleted == false && a.ArticleCateSub == articleCateSub;

            var t = Repository.Entities.FirstOrDefault(predicate);//.ToList().FirstOrDefault();

            return t;
        }

        public List<T> GetListByCode<T>(string Newscode) where T : IViewModel, new()
        {
            Guid strCode = Guid.Parse(Newscode);
            Expression<Func<ArticleInfo, bool>> predicate = a => a.IsDeleted == false && a.ArticleCode == strCode;

            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
        }

        public List<T> GetList<T>(Expression<Func<ArticleInfo, bool>> predicate) where T : IViewModel, new()
        {

            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();


            //更新CategoryCode
            //var lstCate = CommonService.GetCategory(CategoryType.ArticleInfoCate);

            lst.ForEach(d =>
            {
                dynamic a = d;

                //var cate = lstCate.FirstOrDefault(b => b.CategoryCode == a.ArticleCateSub);
                var cate = CommonService.GetCategory((CategoryType)a.AppId, false).FirstOrDefault(b => b.CategoryCode == a.ArticleCateSub);
                if (cate != null)
                {
                    a.ArticleCateName = cate.CategoryName;
                }
            });


            return lst;
        }

        public List<T> GetArticleList<T>(Expression<Func<ArticleInfo, bool>> predicate) where T : IViewModel, new()
        {

            var lst =
                Repository.Entities.Where(predicate).OrderByDescending(m => m.CreatedDate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return lst;
        }

        public override List<T> GetList<T>(Expression<Func<ArticleInfo, bool>> predicate,
           int pageIndex,
           int pageSize,
           ref int total,
          List<SortCondition> sortConditions = null)
        {
            if (total <= 0)
            {
                total = Repository.Entities.Count(predicate);
            }
            var source = Repository.Entities.Where(predicate);

            if (sortConditions == null || sortConditions.Count == 0)
            {
                source = source.OrderByDescending(m => m.Id);
            }
            else
            {
                int count = 0;
                IOrderedQueryable<ArticleInfo> orderSource = null;
                foreach (SortCondition sortCondition in sortConditions)
                {
                    orderSource = count == 0
                        ? CollectionPropertySorter<ArticleInfo>.OrderBy(source, sortCondition.SortField, sortCondition.ListSortDirection)
                        : CollectionPropertySorter<ArticleInfo>.ThenBy(orderSource, sortCondition.SortField, sortCondition.ListSortDirection);
                    count++;
                }
                source = orderSource;
            }
            var lst = source != null
                ? source.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                : Enumerable.Empty<ArticleInfo>();

            return lst.ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            //  var lst = this.Entities.Where(predicate).Take(iTop).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            // return lst;
        }

        public override int InsertView<T>(T objModalSrc)
        {

            return AddOrUpdate(objModalSrc, true, default(List<string>));

        }

        public override int UpdateView<T>(T objModalSrc)
        {

            return AddOrUpdate(objModalSrc, false, null);

        }

        public override int UpdateView<T>(T objModalSrc, List<string> lst)
        {

            return AddOrUpdate(objModalSrc, false, lst);

        }

        private int AddOrUpdate<T>(T objModalSrc, bool bolAdd, List<string> lst)
        {
            ArticleInfoView objView = objModalSrc as ArticleInfoView;
            if (objView == null)
            {
                return -1;
            }
            int iRet;

            BaseService<ArticleImages> ser = new BaseService<ArticleImages>("CAAdmin");

            foreach (var a in objView.ArticleContentViews)
            {
                if (a.objImage != null)
                {
                    ser.Repository.Insert(a.objImage);
                    a.ImageID = a.objImage.Id;
                }
            }

            var article = objView.MapTo<ArticleInfo>();
            // 处理article.content
            article.ArticleContentEdit = JsonHelper.ToJson(objView.ArticleContentViews);

            if (bolAdd)
            {
                article.ArticleStatus = ConstData.STATUS_NEW;
                article.ReadCount = 0;
                article.ThumbsUpCount = 0;
                iRet = Repository.Insert(article);
                objView.Id = article.Id;
            }
            else
            {
                if (lst == null || lst.Count == 0)
                {
                    iRet = Repository.Update(article);
                }
                else
                {
                    iRet = Repository.Update(article, lst);
                }
            }

            foreach (var a in objView.ArticleContentViews)
            {
                if (a.objImage != null)
                {
                    a.objImage.ArticleID = article.Id;
                    ser.Repository.Update(a.objImage, new List<string>() { "ArticleID" });
                }
            }

            return iRet;
        }

        public int UpdateArticleThumbsUp(int articleId, string userId, string type)
        {
            var likedUsers = _articleThumbsUpService.Repository.Entities.Where(x => x.ArticleID == articleId && x.Type == type).Select(x => new { UserID = x.UserID, Id = x.Id, IsDelete = x.IsDeleted }).ToList();

            var currentUser = likedUsers.FirstOrDefault(x => x.UserID == userId);

            var likedCount = likedUsers.Count(x => x.IsDelete != true);

            if (currentUser != null)
            {
                if (currentUser.IsDelete == true)
                {
                    _articleThumbsUpService.Repository.Update(new ArticleThumbsUp { Id = currentUser.Id, IsDeleted = false }, new List<string> { "IsDeleted" });
                    likedCount++;
                }
                else
                {
                    _articleThumbsUpService.Repository.Update(new ArticleThumbsUp { Id = currentUser.Id, IsDeleted = true }, new List<string> { "IsDeleted" });
                    likedCount--;
                }

            }
            else
            {
                _articleThumbsUpService.Repository.Insert(new ArticleThumbsUp
                {
                    ArticleID = articleId,
                    UserID = userId,
                    Type = type
                });
                likedCount++;
            }

            return likedCount;
        }

        public override List<T> GetList<T>(Expression<Func<ArticleInfo, bool>> func, PageCondition page)
        {
            var total = 0;

            var list = GetList<ArticleInfoView>(func, page.PageIndex, page.PageSize, ref  total, page.SortConditions);
            var ids = list.AsParallel().Select(x => x.Id).ToList();
            var thumbsups = _articleThumbsUpService.Repository.Entities.Where(x => ids.Contains(x.ArticleID) &&
                x.IsDeleted != true && x.Type == ThumbupType.Article.ToString()).Select(x => new { x.ArticleID }).ToList().AsParallel();
            page.RowCount = total;

            Parallel.ForEach(list, x =>
            {
                x.ThumbsUpCount = thumbsups.Count(y => y.ArticleID == x.Id);
            });

            return list.Select(x => (T)(IViewModel)x).ToList();
        }

        private static IQueryable<ArticleLikeEntity> GetArticleInfosWithThumbsup(IQueryable<ArticleInfo> queryable)
        {
            // return queryable.Select(x => new
            //ArticleLikeEntity
            //{
            //    ArticleInfo = x,
            //    ArticleThumbsUps = x.ArticleThumbsUps.Where(y => y.ArticleID
            //        == x.Id && y.IsDeleted != true).ToList()
            //});

            throw new NotImplementedException();
        }

        public string EncryptorLillyid(string lillyId)
        {
            return EncryptionHelper.Encrypt(lillyId, CommonService.lstSysConfig.First(x => x.ConfigName == keyName).ConfigValue);
        }

        public string DecryptLillyid(string encryLillyId)
        {
            return EncryptionHelper.Decrypt(encryLillyId, CommonService.lstSysConfig.First(x => x.ConfigName == keyName).ConfigValue);
        }

        public ArticleInfo GetArticleFromCache(int id, int? expirationTime = null)
        {
            var key = string.Format(CacheKeyPrefix, id);

            return _cacheManager.Get(key, expirationTime ?? 1, () => Repository.GetByKey(id));
        }

        public void BackendUpdateReadCount(int id)
        {
            Task.Factory.StartNew(x =>
            {
                var log = Infrastructure.Core.Logging.LogManager.GetLogger(typeof(ArticleInfoService).Name);
                try
                {
                    var articleService = EngineContext.Current.Resolve<IArticleInfoService>();
                    articleService.Repository.SqlExcute("UPDATE dbo.ArticleInfo SET ReadCount=ReadCount+1 WHERE Id=@id",
                        new SqlParameter("@id", (int)x));
                }
                catch (Exception e)
                {
                    log.Error(e, "update articleinfo readcount");
                }
            }, id);
        }
    }

    public class ArticleLikeEntity
    {
        public ArticleInfo ArticleInfo { get; set; }

        public IList<ArticleThumbsUp> ArticleThumbsUps { get; set; }
    }

    public enum ThumbupType
    {
        Article,
        Message
    }
}