/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB

    文件名：JsApiTicketContainer.cs
    文件功能描述：通用接口JsApiTicket容器，用于自动管理JsApiTicket，如果过期会重新获取


    创建标识：DLYB - 20160206

    修改标识：DLYB - 20160206
    修改描述：将public object Lock更改为internal object Lock

    修改标识：DLYB - 20160318
    修改描述：13.6.10 使用FlushCache.CreateInstance使注册过程立即生效

----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using DLYB.Weixin.Containers;
using DLYB.Weixin.Exceptions;
using DLYB.Weixin.MP.Entities;
using DLYB.Weixin.CacheUtility;
using DLYB.Weixin.Entities;

namespace DLYB.Weixin.MP.CommonAPIs
{
    ///// <summary>
    ///// JsApiTicket包
    ///// </summary>
    //[Serializable]
    //public class JsApiTicketBag : BaseContainerBag<JsApiTicketResult>
    //{


    //    //public JsApiTicketResult JsApiTicketResult
    //    //{
    //    //    get;
    //    //    set;
    //    //}



    //    /// <summary>
    //    /// 只针对这个AppId的锁
    //    /// </summary>
    //   // internal object Lock = new object();

    //}

    /// <summary>
    /// 通用接口JsApiTicket容器，用于自动管理JsApiTicket，如果过期会重新获取
    /// </summary>
    public class JsApiTicketContainer 
    {
        //static Dictionary<string, JsApiTicketBag> JsApiTicketCollection =
        //   new Dictionary<string, JsApiTicketBag>(StringComparer.OrdinalIgnoreCase);

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
        /// 返回已经注册的第一个AppId
        /// </summary>
        /// <returns></returns>
        public static string GetFirstOrDefaultAppId()
        {
            return null;// ItemCollection.GetAll().Keys.FirstOrDefault();
        }

        #region JsApiTicket

        /// <summary>
        /// 使用完整的应用凭证获取Ticket，如果不存在将自动注册
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="getNewTicket"></param>
        /// <returns></returns>
        public static string TryGetJsApiTicket(string appId, string appSecret, bool getNewTicket = false)
        {
            return DLYB.Weixin.CommonAPIs.JsApiTicketContainer.GetJsApiTicket(appId, appSecret, CommonApi.GetTicket, getNewTicket);
        }

        /// <summary>
        /// 获取可用Ticket
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="getNewTicket">是否强制重新获取新的Ticket</param>
        /// <returns></returns>
        public static string GetJsApiTicket(string appId, string corpSecret, bool getNewTicket = false)
        {
            return GetJsApiTicketResult(appId,corpSecret, getNewTicket).ticket;
        }

        /// <summary>
        /// 获取可用Ticket
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="getNewTicket">是否强制重新获取新的Ticket</param>
        /// <returns></returns>
        public static JsApiTicketResult GetJsApiTicketResult(string appId,string corpSecret, bool getNewTicket = false)
        {


            return DLYB.Weixin.CommonAPIs.JsApiTicketContainer.GetJsApiTicketResult(appId, corpSecret, CommonApi.GetTicket, getNewTicket);

            //if (!CheckRegistered(appId))
            //{
            //    throw new UnRegisterAppIdException(null, "此appId尚未注册，请先使用JsApiTicketContainer.Register完成注册（全局执行一次即可）！");
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

        #endregion

    }
}
