// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
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
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Service.Common;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Autofac;
using Infrastructure.Web.Domain.Service;
using Novell.Directory.Ldap.Utilclass;


namespace Infrastructure.Web.Domain.Services
{
    /// <summary>
    /// 业务实现——身份认证模块
    /// </summary>
    public partial class CategoryService : BaseService<Category>, ICategoryService
    {
        private static ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(CommonService)));

        public List<T> GetListByCode<T>(string code) where T : IViewModel, new()
        {
            Expression<Func<Category, bool>> predicate = a => a.IsDeleted == false && a.CategoryCode == code;

            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
        }

        public  int Delete(Int32[] IDs)
        {
            var lst = Repository.Entities.Where(a => IDs.Contains(a.Id)).Select(a => a.CategoryCode);
            int result = Repository.Delete(a => lst.Contains(a.CategoryCode));
            cacheManager.Remove("Category");
            return result;
        }


        /// InsertView
        /// </summary>
        /// <param name="objModal"></param>
        /// <returns></returns>
        public override int InsertView<T>(T objModalSrc)
        {
            int iRet;
            var objModal = (CategoryView)(IViewModel)objModalSrc;
            
            Category obj = new Category();
            obj = mapObject(objModal, obj, false);
            obj.IsDeleted = false;
            iRet = Repository.Insert(obj);

            cacheManager.Remove("Category");

            return iRet;
        }

        public override int UpdateView<T>(T objModalSrc)
        {
            int iRet = 0;
            var objModal = (CategoryView)(IViewModel)objModalSrc;
            Category obj = new Category();
            obj = mapObject(objModal, obj, false);
            iRet = Repository.Update(obj);

            cacheManager.Remove("Category");

            return iRet;
        }

        public Category mapObject(CategoryView objModal, Category obj, bool IsEnglish)
        {
            if (IsEnglish)
            {
                obj.LanguageCode = ConstData.LAN_EN;
                obj.CategoryName = objModal.CategoryNameCN;
            }
            else
            {
                obj = objModal.MapTo<Category>();
                obj.LanguageCode = ConstData.LAN_CN;
                obj.CategoryName = objModal.CategoryName;
            }

            //obj.Extra1 = objModal.Extra1;
            obj.NoRoleMessage = objModal.NoRoleMessage;
            obj.AppId = objModal.AppId;
            obj.CategoryCode = objModal.CategoryCode;

            return obj;
        }

    }
}