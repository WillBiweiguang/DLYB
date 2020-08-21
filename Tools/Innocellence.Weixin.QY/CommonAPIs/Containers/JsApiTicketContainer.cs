/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB

    文件名：JsApiTicketContainer.cs
    文件功能描述：通用接口JsApiTicket容器，用于自动管理JsApiTicket，如果过期会重新获取


    创建标识：DLYB - 20150313

    修改标识：DLYB - 20150313
    修改描述：整理接口

    修改标识：DLYB - 20160312
    修改描述：升级Container，继承自BaseContainer<JsApiTicketBag>

    修改标识：DLYB - 20160318
    修改描述：v3.3.4 使用FlushCache.CreateInstance使注册过程立即生效

----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using DLYB.Weixin.CacheUtility;
using DLYB.Weixin.Containers;
using DLYB.Weixin.Exceptions;
using DLYB.Weixin.QY.Entities;
using DLYB.Weixin.QY.Exceptions;
using DLYB.Weixin.Entities;

namespace DLYB.Weixin.QY.CommonAPIs
{
    ///// <summary>
    ///// JsApiTicketBag
    ///// </summary>
    //[Serializable]
    //public class JsApiTicketBag : BaseContainerBag<JsApiTicketResult>
    //{


    //    public JsApiTicketResult JsApiTicketResult
    //    {
    //        get;
    //        set;
    //    }


    //    /// <summary>
    //    /// 只针对这个AppId的锁
    //    /// </summary>
    //    internal object Lock = new object();

    //}

    /// <summary>
    /// 通用接口JsApiTicket容器，用于自动管理JsApiTicket，如果过期会重新获取
    /// </summary>
    public class JsApiTicketContainer 
    {
        //private const string UN_REGISTER_ALERT = "此AppId尚未注册，JsApiTicketContainer.Register完成注册（全局执行一次即可）！";

        ///// <summary>
        ///// 注册应用凭证信息，此操作只是注册，不会马上获取Ticket，并将清空之前的Ticket，
        ///// </summary>
        ///// <param name="appId"></param>
        ///// <param name="appSecret"></param>
        //public static void Register(string appId, string appSecret)
        //{
        //    using (FlushCache.CreateInstance())
        //    {
        //        Update(appId, new JsApiTicketBag()
        //        {
        //             CorpId = appId,
        //             CorpSecret = appSecret,
        //            ExpireTime = DateTime.MinValue,
        //            JsApiTicketResult = new JsApiTicketResult()
        //        });
        //    }
        //}

        /// <summary>
        /// 使用完整的应用凭证获取Ticket，如果不存在将自动注册
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="getNewTicket"></param>
        /// <returns></returns>
        public static string TryGetTicket(string appId, string appSecret, bool getNewTicket = false)
        {
            return DLYB.Weixin.CommonAPIs.JsApiTicketContainer.GetJsApiTicket(appId, appSecret, CommonApi.GetTicket, getNewTicket);
        }

        /// <summary>
        /// 获取可用Ticket
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="getNewTicket">是否强制重新获取新的Ticket</param>
        /// <returns></returns>
        public static string GetTicket(string appId, string corpSecret, bool getNewTicket = false)
        {
            return GetTicketResult(appId,corpSecret, getNewTicket).ticket;
        }

        /// <summary>
        /// 获取可用Ticket
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="getNewTicket">是否强制重新获取新的Ticket</param>
        /// <returns></returns>
        public static JsApiTicketResult GetTicketResult(string appId,string corpSecret, bool getNewTicket = false)
        {


            return DLYB.Weixin.CommonAPIs.JsApiTicketContainer.GetJsApiTicketResult(appId, corpSecret, CommonApi.GetTicket, getNewTicket);


            //if (!CheckRegistered(appId))
            //{
            //    throw new WeixinQyException(UN_REGISTER_ALERT);
            //}

            //var jsApiTicketBag = (JsApiTicketBag)GetItem(appId);
            //lock (jsApiTicketBag.Lock)
            //{
            //    if (getNewTicket || jsApiTicketBag.ExpireTime <= DateTime.Now)
            //    {
            //        //已过期，重新获取
            //        jsApiTicketBag.JsApiTicketResult = CommonApi.GetTicket(jsApiTicketBag.CorpId, jsApiTicketBag.CorpSecret);
            //        jsApiTicketBag.ExpireTime = DateTime.Now.AddSeconds(jsApiTicketBag.JsApiTicketResult.expires_in);
            //    }
            //}
            //return jsApiTicketBag.JsApiTicketResult;
        }

        ///// <summary>
        ///// 检查是否已经注册
        ///// </summary>
        ///// <param name="appId"></param>
        ///// <returns></returns>
        //public new static bool CheckRegistered(string appId)
        //{
        //    return true;// ItemCollection.CheckExisted(appId);
        //}
    }
}
