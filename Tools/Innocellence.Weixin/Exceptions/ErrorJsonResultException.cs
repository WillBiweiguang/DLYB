﻿/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：ErrorJsonResultException.cs
    文件功能描述：JSON返回错误代码（比如token_access相关操作中使用）。
    
    
    创建标识：DLYB - 20150211
    
    修改标识：DLYB - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using DLYB.Weixin.Entities;
using DLYB.Weixin.Logging;

namespace DLYB.Weixin.Exceptions
{
    /// <summary>
    /// JSON返回错误代码（比如token_access相关操作中使用）。
    /// </summary>
    public class ErrorJsonResultException : WeixinException
    {
        public WxJsonResult JsonResult { get; set; }
        public string Url { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        /// <param name="jsonResult"></param>
        /// <param name="url"></param>
        public ErrorJsonResultException(string message, Exception inner, WxJsonResult jsonResult, string url = null)
            : base(message, inner)
        {
            JsonResult = jsonResult;
            Url = url;

            Log4NetLog.Logger.Error("ErrorJsonResultException", this);

            WeixinTrace.ErrorJsonResultExceptionLog(this);
        }
    }
}
