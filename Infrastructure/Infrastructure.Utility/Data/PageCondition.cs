// -----------------------------------------------------------------------
//  <copyright file="PageCondition.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 0:08</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Infrastructure.Utility.Data
{
    /// <summary>
    /// 分页查询条件信息
    /// </summary>
    public class PageCondition
    {
        /// <summary>
        /// 初始化一个 默认参数（第1页，每页20，排序条件为空）的分页查询条件信息类 的新实例
        /// </summary>
        public PageCondition()
        {
            PageIndex = 1;
            PageSize = 20;
            SortConditions = new List<SortCondition> ();
        }

        /// <summary>
        /// 初始化一个 指定页索引与页大小的分页查询条件信息类 的新实例
        /// </summary>
        /// <param name="pageIndex"> 页索引 </param>
        /// <param name="pageSize"> 页大小 </param>
        public PageCondition(int pageIndex, int pageSize)
            : this()
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        /// <summary>
        /// 获取或设置 页索引
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 获取或设置 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// 获取或设置 排序条件组
        /// </summary>
        public List<SortCondition> SortConditions { get; set; }
    }
}