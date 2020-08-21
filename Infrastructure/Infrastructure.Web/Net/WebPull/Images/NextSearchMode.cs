// -----------------------------------------------------------------------
//  <copyright file="NextSearchMode.cs" company="DLYB">
//      Copyright (c) 2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 12:29</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Infrastructure.Web.Net.WebPull.Images
{
    /// <summary>
    /// 下一个数据搜索模式
    /// </summary>
    public enum NextSearchMode
    {
        /// <summary>
        /// 循环模式，知道总数，使用循环遍历
        /// </summary>
        Cycle,

        /// <summary>
        /// 逐个查询，从当前数据中获取下一数据的标识，一个一个获取
        /// </summary>
        OneByOne
    }
}