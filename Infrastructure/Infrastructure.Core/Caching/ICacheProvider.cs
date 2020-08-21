// -----------------------------------------------------------------------
//  <copyright file="ICacheProvider.cs" company="Innocellence">
//      Copyright (c) 2014-2015 Innocellence. All rights reserved.
//  </copyright>
//  <last-editor>@Innocellence</last-editor>
//  <last-date>2015-04-22 15:36</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infrastructure.Core.Caching
{
    /// <summary>
    /// 缓存提供者约定，用于创建并管理缓存对象
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// 获取 缓存是否可用
        /// </summary>
        bool Enabled { get; } 

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="regionName">缓存区域名称</param>
        /// <returns></returns>
        ICache GetCache(string regionName);

    }
}