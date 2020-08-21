// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="Innocellence">
//      Copyright (c) 2014-2015 Innocellence. All rights reserved.
//  </copyright>
//  <last-editor>@Innocellence</last-editor>
//  <last-date>2015-04-22 17:21</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Contracts;
using System.Linq.Expressions;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.Domain.Service.Common;
using Infrastructure.Web.Domain.ModelsView;
using System.IO;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;


namespace Infrastructure.Web.Domain.Services
{
    /// <summary>
    /// 业务实现——新闻
    /// </summary>
    public partial class ArticleInfoService : BaseService<ArticleInfo>, IArticleInfoService
    {
       // private IImageInfoService _imageService;
        public ArticleInfoService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
           // _imageService = imageService;
        }

        public ArticleInfoService()
            : base()
        {
            
        }

        public T GetById<T>(int id) where T : IViewModel, new()
        {
            Expression<Func<ArticleInfo, bool>> predicate = a => a.IsDeleted == false && a.Id == id;

            var t = this.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).FirstOrDefault();

            return t;
        }

        public List<T> GetListByCode<T>(string Newscode) where T : IViewModel, new()
        {
            Guid strCode = Guid.Parse(Newscode);
            Expression<Func<ArticleInfo, bool>> predicate = a => a.IsDeleted == false && a.ArticleCode == strCode;

            var lst = this.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
        }

        public List<T> GetList<T>(Expression<Func<ArticleInfo, bool>> predicate) where T : IViewModel, new()
        {

            var lst = this.Entities.Where(predicate).ToList().Select(a => new ArticleInfo
            {
                Id = a.Id,
                //  ImageName = a.ImageName,
                LanguageCode = a.LanguageCode,
                ArticleCate = a.ArticleCate,
                ArticleCode = a.ArticleCode,
                ArticleComment = a.ArticleComment,
                ImageContent_T = a.ImageContent_T,
                ArticleContent = a.ArticleContent,
                ArticleStatus = a.ArticleStatus,
                ArticleTitle = a.ArticleTitle,
                PublishDate = a.PublishDate,
                CreatedDate = a.CreatedDate
            }).Select(n => (T)(new T().ConvertAPIModel(n))).ToList();


            //更新CategoryCode
            var lstCate = CommonService.GetCategory(CategoryType.ArticleInfoCate);

            lst.ForEach(d =>
            {
                dynamic a = d;
                var cate = lstCate.FirstOrDefault(b => b.CategoryCode == a.NewsCate);
                if (cate != null)
                {
                    a.NewsCateName = cate.CategoryName;
                }
            });


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
                total = Entities.Count(predicate);
            }
            var source = Entities.Where(predicate);

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
          return  AddOrUpdate(objModalSrc,true);

        }

        public override int UpdateView<T>(T objModalSrc)
        {


            return AddOrUpdate(objModalSrc, false);


            //int iRet = 0;
            //ArticleInfoView objView = objModalSrc as ArticleInfoView;
            //if (objView == null)
            //{
            //    return -1;
            //}

            //var article = new ArticleInfo();

            //article = objView.MapTo<ArticleInfo>();
            //// 处理article.content
            //article.ArticleContent = JsonHelper.ToJson(objView.ArticleContentStructures);

            //iRet = base.Update(article);
            //return iRet;
        }


        private int AddOrUpdate<T>(T objModalSrc,bool bolAdd)
        {
            ArticleInfoView objView = objModalSrc as ArticleInfoView;
            if (objView == null)
            {
                return -1;
            }
            int iRet;

            BaseService<ArticleImages> ser = new BaseService<ArticleImages>();

            // article = new ArticleInfo();

            foreach (var a in objView.ArticleContentViews)
            {
                if (a.objImage != null)
                {
                    ser.Insert(a.objImage);
                    a.ImageID = a.objImage.Id;
                }
            }

            var article = objView.MapTo<ArticleInfo>();
            // 处理article.content
            article.ArticleContentEdit = JsonHelper.ToJson(objView.ArticleContentViews);

            if (bolAdd) {
            iRet = base.Insert(article);
            }
            else
            {
                iRet = base.Update(article);

            }
            foreach (var a in objView.ArticleContentViews)
            {
                if (a.objImage != null)
                {
                    a.objImage.ArticleID = article.Id;
                    ser.Update(a.objImage, new List<string>() {"ArticleID" });
                }
            }


            return iRet;

        }


    }
}