
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrastructure.Utility.Data;


namespace Infrastructure.Core
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseService<TEntity> where TEntity : IEntity
    {


        /// <summary>
        /// 插入viewmodel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        int InsertView<T>(T obj) where T : IViewModel;

        /// <summary>
        /// 更新viewmodel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        int UpdateView<T>(T obj) where T : IViewModel;

        int UpdateView<T>(T obj, List<string> lst) where T : IViewModel;

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iTop"></param>
        /// <param name="predicate"></param>
        /// <param name="sortConditions"></param>
        /// <returns></returns>
        List<T> GetList<T>(int iTop, Expression<Func<TEntity, bool>> predicate, List<SortCondition> sortConditions = null) where T : IViewModel, new();
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <param name="sortConditions"></param>
        /// <returns></returns>
        List<T> GetList<T>(Expression<Func<TEntity, bool>> predicate,
            int pageIndex,
            int pageSize,
            ref int total,
            List<SortCondition> sortConditions = null) where T : IViewModel, new();

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="con"></param>
        /// <param name="sortConditions"></param>
        /// <returns></returns>
        List<T> GetList<T>(Expression<Func<TEntity, bool>> predicate,
          PageCondition con) where T : IViewModel, new();

        /// <summary>
        /// 
        /// </summary>
        IRepository<TEntity, int> Repository { get; set; }
    }
}
