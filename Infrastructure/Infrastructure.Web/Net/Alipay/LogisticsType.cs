// -----------------------------------------------------------------------
//  <copyright file="LogisticsType.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 19:48</last-date>
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace Infrastructure.Web.Net.Alipay
{
    /// <summary>
    /// 表示物流类型的枚举
    /// </summary>
    public enum LogisticsType
    {
        /// <summary>
        /// 平邮
        /// </summary>
        POST = 0,

        /// <summary>
        /// 快递
        /// </summary>
        EXPRESS = 1,

        /// <summary>
        /// EMS
        /// </summary>
        EMS = 2,

        /// <summary>
        /// 无需物流，在发货时使用
        /// </summary>
        DIRECT = 3
    }
}