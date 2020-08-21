// -----------------------------------------------------------------------
//  <copyright file="ILoggerAdapter.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 22:14</last-date>
// -----------------------------------------------------------------------

using System;


namespace Infrastructure.Core.Logging
{
    /// <summary>
    /// 定义日志输出适配器的方法
    /// </summary>
    public interface ILoggerAdapter
    {
        /// <summary>
        /// 由指定类型获取<see cref="ILog"/>日志实例
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <returns></returns>
        ILog GetLogger(Type type);

        /// <summary>
        /// 由指定名称获取<see cref="ILog"/>日志实例
        /// </summary>
        /// <param name="name">指定名称</param>
        /// <returns></returns>
        ILog GetLogger(string name);
    }
}