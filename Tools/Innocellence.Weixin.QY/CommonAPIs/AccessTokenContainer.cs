/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：AccessTokenContainer.cs
    文件功能描述：通用接口AccessToken容器，用于自动管理AccessToken，如果过期会重新获取
    
    
    创建标识：Innocellence - 20130313
    
    修改标识：Innocellence - 20130313
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innocellence.Weixin.CommonAPIs;
using Innocellence.Weixin.Exceptions;
using Innocellence.Weixin.QY.Entities;
using System.Web.Configuration;

namespace Innocellence.Weixin.QY.CommonAPIs
{
    //class AccessTokenBag
    //{
    //    public string CorpId { get; set; }
    //    public string CorpSecret { get; set; }
    //    public DateTime ExpireTime { get; set; }
    //    public AccessTokenResult AccessTokenResult { get; set; }
    //    /// <summary>
    //    /// 只针对这个CorpId的锁
    //    /// </summary>
    //    public object Lock = new object();
    //}




    /// <summary>
    /// 通用接口AccessToken容器，用于自动管理AccessToken，如果过期会重新获取
    /// </summary>
    public class AccessTokenContainer
    {
        
        //static Dictionary<string, AccessTokenBag> AccessTokenCollection =
        //   new Dictionary<string, AccessTokenBag>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 注册应用凭证信息，此操作只是注册，不会马上获取Token，并将清空之前的Token，
        /// </summary>
        /// <param name="corpId"></param>
        /// <param name="corpSecret"></param>
        //public static void Register(string corpId, string corpSecret)
        //{
        //    AccessTokenCollection[corpId] = new AccessTokenBag()
        //    {
        //        CorpId = corpId,
        //        CorpSecret = corpSecret,
        //        ExpireTime = DateTime.MinValue,
        //        AccessTokenResult = new AccessTokenResult()
        //    };
        //}

        /// <summary>
        /// 使用完整的应用凭证获取Token，如果不存在将自动注册
        /// </summary>
        /// <param name="corpId"></param>
        /// <param name="corpSecret"></param>
        /// <param name="getNewToken"></param>
        /// <returns></returns>
        public static string TryGetToken(string corpId, string corpSecret, bool getNewToken = false)
        {
            //if (!CheckRegistered(corpId) || getNewToken)
            //{
            //    Register(corpId, corpSecret);
            //}
            //return GetToken(corpId);

            var strDBFlag = System.Configuration.ConfigurationManager.AppSettings["TokenStoreType"];
           // string strDBFlag = "";

            if (!string.IsNullOrEmpty(strDBFlag) && strDBFlag=="Database")
            {
              return   Innocellence.Weixin.CommonAPIs.AccessTokenDBContainer.TryGetToken(corpId, corpSecret, CommonApi.GetToken, getNewToken);
            }
            else if (!string.IsNullOrEmpty(strDBFlag) && strDBFlag == "Server")
            {
                var strUrlTemp = System.Configuration.ConfigurationManager.AppSettings["TokenServer"];
                string strUrl = string.Format("{0}?corpId={1}&corpSecret={2}", strUrlTemp,corpId, corpSecret);
                var objToken=  Innocellence.Weixin.HttpUtility.Get.GetJson<TokenEntity>(strUrl);
                return objToken.Token;

            }else
            {
                return Innocellence.Weixin.CommonAPIs.AccessTokenContainer.TryGetToken(corpId, corpSecret, CommonApi.GetToken, getNewToken);
            }
        }

        ///// <summary>
        ///// 获取可用Token
        ///// </summary>
        ///// <param name="corpId"></param>
        ///// <param name="getNewToken">是否强制重新获取新的Token</param>
        ///// <returns></returns>
        //public static string GetToken(string corpId, bool getNewToken = false)
        //{
        //    return GetTokenResult(corpId, getNewToken).access_token;
        //}

        ///// <summary>
        ///// 获取可用Token
        ///// </summary>
        ///// <param name="corpId"></param>
        ///// <param name="getNewToken">是否强制重新获取新的Token</param>
        ///// <returns></returns>
        //public static AccessTokenResult GetTokenResult(string corpId, bool getNewToken = false)
        //{
        //    if (!AccessTokenCollection.ContainsKey(corpId))
        //    {
        //        throw new WeixinException("此CorpId尚未注册，请先使用AccessTokenContainer.Register完成注册（全局执行一次即可）！");
        //    }

        //    var accessTokenBag = AccessTokenCollection[corpId];
        //    lock (accessTokenBag.Lock)
        //    {
        //        if (getNewToken || accessTokenBag.ExpireTime <= DateTime.Now)
        //        {
        //            //已过期，重新获取
        //            accessTokenBag.AccessTokenResult = CommonApi.GetToken(accessTokenBag.CorpId,
        //                accessTokenBag.CorpSecret);
        //            accessTokenBag.ExpireTime = DateTime.Now.AddSeconds(7200);
        //        }
        //    }
        //    return accessTokenBag.AccessTokenResult;
        //}

        ///// <summary>
        ///// 检查是否已经注册
        ///// </summary>
        ///// <param name="corpId"></param>
        ///// <returns></returns>
        //public static bool CheckRegistered(string corpId)
        //{
        //    return AccessTokenCollection.ContainsKey(corpId);
        //}
    }

    public class TokenEntity{

        public string Token { get; set; }
        public string ErrorMsg { get; set; }
    }
}
