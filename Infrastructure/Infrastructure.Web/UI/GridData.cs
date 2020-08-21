// -----------------------------------------------------------------------
//  <copyright file="GridData.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 21:45</last-date>
// -----------------------------------------------------------------------

using System.Collections.Generic;


namespace Infrastructure.Web.UI
{
    /// <summary>
    /// 列表数据，封装列表的行数据与总记录数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridData<T>
    {
        /// <summary>
        /// 初始化一个<see cref="GridData{T}"/>类型的新实例
        /// </summary>
        public GridData()
            : this(new List<T>(), 0)
        { }

        /// <summary>
        /// 初始化一个<see cref="GridData{T}"/>类型的新实例
        /// </summary>
        public GridData(IEnumerable<T> rows, int total)
        {
            Rows = rows;
            Total = total;
        }

        /// <summary>
        /// 获取或设置 行数据
        /// </summary>
        public IEnumerable<T> Rows { get; set; }

        /// <summary>
        /// 获取或设置 数据行数
        /// </summary>
        public int Total { get; set; }
    }
}