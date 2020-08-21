// -----------------------------------------------------------------------
//  <copyright file="AjaxResultType.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 9:24</last-date>
// -----------------------------------------------------------------------

using Infrastructure.Utility.Data;


namespace Infrastructure.Web.UI
{
    /// <summary>
    /// 表示 ajax 操作结果类型的枚举
    /// </summary>
    public enum AjaxResultType
    {
        /// <summary>
        /// 消息结果类型
        /// </summary>
        Info,

        /// <summary>
        /// 成功结果类型
        /// </summary>
        Success,

        /// <summary>
        /// 警告结果类型
        /// </summary>
        Warning,

        /// <summary>
        /// 异常结果类型
        /// </summary>
        Error
    }
}