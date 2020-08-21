/*----------------------------------------------------------------
    Copyright (C) 2015 DLYB
    
    文件名：AccessTokenResult.cs
    文件功能描述：access_token请求后的JSON返回格式
    
    
    创建标识：DLYB - 20150211
    
    修改标识：DLYB - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using DLYB.Weixin.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLYB.Weixin.Entities
{

    /// <summary>
    /// JsApiTicket包
    /// </summary>
    [Serializable]
    public class JsApiTicketBag : BaseContainerBag<JsApiTicketResult>
    {

    }

    /// <summary>
    /// AccessToken包
    /// </summary>
    [Serializable]
    public class AccessTokenBag : BaseContainerBag<AccessTokenResult>
    {


    }

    public abstract class BaseTokenResult : WxJsonResult
    {

        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int expires_in { get; set; }
    }

    /// <summary>
    /// access_token请求后的JSON返回格式
    /// </summary>
    public class AccessTokenResult : BaseTokenResult
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string access_token { get; set; }
    }
}
