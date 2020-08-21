/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：WeixinOpenException.cs
    文件功能描述：微信开放平台异常处理类
    
    
    创建标识：DLYB - 20151004

----------------------------------------------------------------*/

using System;
using DLYB.Weixin.Exceptions;
using DLYB.Weixin.QY.CommonAPIs;
using DLYB.Weixin.CommonAPIs;
using DLYB.Weixin.Entities;

namespace DLYB.Weixin.QY.Exceptions
{
    /// <summary>
    /// 企业号异常
    /// </summary>
    public class WeixinQyException : WeixinException
    {
        public AccessTokenBag AccessTokenBag { get; set; }

        public WeixinQyException(string message, AccessTokenBag accessTokenBag = null, Exception inner=null)
            : base(message, inner)
        {
            AccessTokenBag = accessTokenBag;
        }
    }
}
