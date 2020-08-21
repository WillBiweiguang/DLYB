/*----------------------------------------------------------------
    Copyright (C) 2015 DLYB
    
    文件名：AccessTokenContainer.cs
    文件功能描述：通用接口AccessToken容器，用于自动管理AccessToken，如果过期会重新获取
    
    
    创建标识：DLYB - 20150211
    
    修改标识：DLYB - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DLYB.Weixin.Exceptions;
using DLYB.Weixin.Entities;
using DLYB.Weixin.Containers;


namespace DLYB.Weixin.CommonAPIs
{


    /// <summary>
    /// 通用接口AccessToken容器，用于自动管理AccessToken，如果过期会重新获取
    /// </summary>
    public class JsApiTicketContainer : IBaseContainer
    {
        static Dictionary<string, JsApiTicketBag> AccessTokenCollection =
           new Dictionary<string, JsApiTicketBag>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 注册应用凭证信息，此操作只是注册，不会马上获取Token，并将清空之前的Token，
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        public static void Register(string appId, string appSecret)
        {
            string strKey = appId + "_" + appSecret + "_" + typeof(JsApiTicketBag).Name;

            AccessTokenCollection[strKey] = new JsApiTicketBag()
            {
                CorpId = appId,
                CorpSecret = appSecret,
                ExpireTime = DateTime.MinValue,
                TokenResult = new JsApiTicketResult()
            };
        }


        public static void UpdateNull(string appId, string appSecret)
        {
            string strKey = appId + "_" + appSecret + "_" + typeof(JsApiTicketBag).Name;

            //var v = new JsApiTicketBag()
            //{
            //    CorpId = appId,
            //    CorpSecret = appSecret,
            //    ExpireTime = DateTime.MinValue,
            //    TokenResult = new JsApiTicketResult()
            //};

            JsApiTicketBag k;
            AccessTokenCollection.TryGetValue(strKey, out k);
            k.ExpireTime = DateTime.MinValue;
        }

        ///// <summary>
        ///// 使用完整的应用凭证获取Token，如果不存在将自动注册
        ///// </summary>
        ///// <param name="appId"></param>
        ///// <param name="appSecret"></param>
        ///// <param name="getNewToken"></param>
        ///// <returns></returns>
        //public static string TryGetToken(string appId, string appSecret, Func<string, string, string, AccessTokenResult> func, bool getNewToken = false)
        //{
        //    if (!CheckRegistered(appId) || getNewToken)
        //    {
        //        Register(appId, appSecret);
        //    }
        //    return GetToken(appId, appSecret, func);
        //}

        /// <summary>
        /// 获取可用Token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="getNewToken">是否强制重新获取新的Token</param>
        /// <returns></returns>
        public static string GetJsApiTicket(string appId, string appSecret, Func<string, string, string, JsApiTicketResult> func, bool getNewToken = false)
        {
            return GetJsApiTicketResult(appId, appSecret, func, getNewToken).ticket;
        }

        /// <summary>
        /// 获取可用Token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="getNewToken">是否强制重新获取新的Token</param>
        /// <returns></returns>
        public static JsApiTicketResult GetJsApiTicketResult(string appId, string appSecret, Func<string, string, string, JsApiTicketResult> func, bool getNewToken = false)
        {
            string strKey = appId + "_" + appSecret + "_" + typeof(JsApiTicketBag).Name;

            if (!CheckRegistered(strKey))
            {
                Register(appId, appSecret);
            }

            var accessTokenBag = AccessTokenCollection[strKey];
            lock (accessTokenBag.Lock)
            {
                if (getNewToken || accessTokenBag.ExpireTime <= DateTime.Now)
                {
                    var strDBFlag = System.Configuration.ConfigurationManager.AppSettings["TicketStoreType"];


                    JsApiTicketBag token = null;

                    if (string.IsNullOrEmpty(strDBFlag) || strDBFlag != "Server")
                    {
                        token = AccessTokenCache<JsApiTicketBag, JsApiTicketResult>.GetAccessTokenResult(appId, appSecret, func, getNewToken);
                    }
                    else
                    {
                        var strUrlTemp = System.Configuration.ConfigurationManager.AppSettings["TicketServer"];
                        string strUrl = string.Format("{0}?corpId={1}&corpSecret={2}", strUrlTemp, appId, appSecret);
                        var objToken = DLYB.Weixin.HttpUtility.Get.GetJson<TicketEntity>(strUrl);
                        token = new JsApiTicketBag()
                        {
                            TokenResult = new JsApiTicketResult() { ticket = objToken.Ticket, expires_in = objToken.expires_in },
                            ExpireTime = objToken.ExpireTime,
                            CorpId = appId,
                            CorpSecret = appSecret
                        };
                    }

                    //已过期，重新获取
                    accessTokenBag.TokenResult = token.TokenResult;
                    accessTokenBag.ExpireTime = token.ExpireTime.AddMinutes(5); //让缓存先过期

                }
            }
            return accessTokenBag.TokenResult;
        }

        /// <summary>
        /// 检查是否已经注册
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckRegistered(string appId)
        {
            return AccessTokenCollection.ContainsKey(appId);
        }
    }

    public class TicketEntity
    {

        public string Ticket { get; set; }
        public string ErrorMsg { get; set; }

        public int expires_in { get; set; }

        public DateTime ExpireTime { get; set; }
    }
}
