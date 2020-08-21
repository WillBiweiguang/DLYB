// -----------------------------------------------------------------------
//  <copyright file="AutoMapperExtension.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 15:02</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

using AutoMapper;


namespace Infrastructure.Core.Data
{
    /// <summary>
    /// AutoMapper 辅助扩展方法
    /// </summary>
    public static class AutoMapperExtension
    {
        /// <summary>
        /// 创建映射，使用源对象创建目标对象
        /// </summary>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns>创建的目标对象</returns>
        public static TTarget MapTo<TTarget>(this object source) where TTarget : IEntity
        {
            return Mapper.Map<TTarget>(source);
        }

        /// <summary>
        /// 更新映射，使用源对象更新目标对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">要更新的目标对象</param>
        /// <returns>更新后的目标对象</returns>
        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget target)
            where TSource : IEntity
            where TTarget : IEntity
        {
            return Mapper.Map<TSource, TTarget>(source, target);
        }
    }
}