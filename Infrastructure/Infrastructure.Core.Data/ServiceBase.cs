// -----------------------------------------------------------------------
//  <copyright file="ServiceBase.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 3:42</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;


namespace Infrastructure.Core.Data
{

    /// <summary>
    /// 业务实现基类
    /// </summary>
    public class BaseService<TEntity> : IBaseService<TEntity>
        where TEntity : EntityBase<int>
    {

        /// <summary>
        /// 
        /// </summary>
        public IRepository<TEntity, int> Repository { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        public BaseService(IUnitOfWork unitOfWork)
        {
            Repository = new Repository<TEntity, int>(unitOfWork);
        }

        /// <summary>
        /// 
        /// </summary>
        public BaseService()
        {
            //TODO 修改为MySql
            //Repository = new Repository<TEntity, int>(new CodeFirstDbContext());
            Repository = new Repository<TEntity, int>(new MySqlDbContext());
        }

        /// <summary>
        /// 构造函数，直接连接串
        /// </summary> : base(new CodeFirstDbContext(strDBName))
        /// <param name="strDBName"></param>
        public BaseService(string strDBName)
        {
            //TODO 修改为MySQL
            //Repository = new Repository<TEntity, int>(new CodeFirstDbContext(strDBName));
            Repository = new Repository<TEntity, int>(new MySqlDbContext(strDBName));
        }

        /// <summary>
        /// 插入viewmodel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual int InsertView<T>(T obj) where T : IViewModel
        {
            var t = obj.MapTo<TEntity>();
            return Repository.Insert(t);
        }

        /// <summary>
        /// 更新viewmodel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual int UpdateView<T>(T obj) where T : IViewModel
        {
            var t = obj.MapTo<TEntity>();
            return Repository.Update(t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        public virtual int UpdateView<T>(T obj, List<string> lst) where T : IViewModel
        {
            var t = obj.MapTo<TEntity>();
            return Repository.Update(t, lst);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iTop"></param>
        /// <param name="predicate"></param>
        /// <param name="sortConditions"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(int iTop, Expression<Func<TEntity, bool>> predicate, List<SortCondition> sortConditions = null) where T : IViewModel, new()
        {

            var source = Repository.Entities.Where(predicate).Take(iTop).AsNoTracking();


            if (sortConditions == null || sortConditions.Count == 0)
            {
                source = source.OrderByDescending(m => m.Id);
            }
            else
            {
                int count = 0;
                IOrderedQueryable<TEntity> orderSource = null;
                foreach (SortCondition sortCondition in sortConditions)
                {
                    orderSource = count == 0
                        ? CollectionPropertySorter<TEntity>.OrderBy(source, sortCondition.SortField, sortCondition.ListSortDirection)
                        : CollectionPropertySorter<TEntity>.ThenBy(orderSource, sortCondition.SortField, sortCondition.ListSortDirection);
                    count++;
                }
                source = orderSource;
            }


            return source != null ? source.ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList() : new List<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="con"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual List<T> GetList<T>(Expression<Func<TEntity, bool>> predicate,
           PageCondition con) where T : IViewModel, new()
        {
            int iTotal = con.RowCount;
            var list = GetList<T>(predicate, con.PageIndex, con.PageSize, ref iTotal, con.SortConditions);

            con.RowCount = iTotal;

            return list;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <param name="sortConditions"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(Expression<Func<TEntity, bool>> predicate,
            int pageIndex,
            int pageSize,
            ref int total,
           List<SortCondition> sortConditions = null) where T : IViewModel, new()
        {


            //source.CheckNotNull("source");
            //predicate.CheckNotNull("predicate");
            //pageIndex.CheckGreaterThan("pageIndex", 0);
            //pageSize.CheckGreaterThan("pageSize", 0);

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
                IOrderedQueryable<TEntity> orderSource = null;
                foreach (SortCondition sortCondition in sortConditions)
                {
                    orderSource = count == 0
                        ? CollectionPropertySorter<TEntity>.OrderBy(source, sortCondition.SortField, sortCondition.ListSortDirection)
                        : CollectionPropertySorter<TEntity>.ThenBy(orderSource, sortCondition.SortField, sortCondition.ListSortDirection);
                    count++;
                }
                source = orderSource;
            }
            var lst = source != null
                ? source.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                : Enumerable.Empty<TEntity>();

            return lst.ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            //  var lst = this.Entities.Where(predicate).Take(iTop).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            // return lst;
        }

    }
}