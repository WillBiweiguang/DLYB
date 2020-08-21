// -----------------------------------------------------------------------
//  <copyright file="AjaxResult.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 1:58</last-date>
// -----------------------------------------------------------------------

namespace Infrastructure.Web.UI
{
    /// <summary>
    /// 表示Ajax操作结果 
    /// </summary>
    public class AjaxResult
    {
        /// <summary>
        /// 初始化一个<see cref="AjaxResult"/>类型的新实例
        /// </summary>
        public AjaxResult(string content, AjaxResultType type = AjaxResultType.Info, object data = null)
            : this(content, data, type)
        { }

        /// <summary>
        /// 初始化一个<see cref="AjaxResult"/>类型的新实例
        /// </summary>
        public AjaxResult(string content, object data, AjaxResultType type = AjaxResultType.Info)
        {
            Type = type.ToString();
            Content = content;
            Data = data;
        }

        /// <summary>
        /// 获取 Ajax操作结果类型
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// 获取 消息内容
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// 获取 返回数据
        /// </summary>
        public object Data { get; private set; }
    }
}