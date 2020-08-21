/*----------------------------------------------------------------
    Copyright (C) 2015 DLYB
    
    文件名：AccessTokenContainer.cs
    文件功能描述：通用接口AccessToken容器，用于自动管理AccessToken，如果过期会重新获取
    
    
    创建标识：DLYB - 20130313
    
    修改标识：DLYB - 20130313
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DLYB.Weixin.Exceptions;
using DLYB.Weixin.QY.Entities;
using DLYB.Weixin.CommonAPIs;
using DLYB.Weixin.Entities;

namespace DLYB.Weixin.QY.CommonAPIs
{
   

    /// <summary>
    /// 通用接口AccessToken容器，用于自动管理AccessToken，如果过期会重新获取
    /// </summary>
    public class AccessTokenContainer
    {
        static Dictionary<string, AccessTokenBag> AccessTokenCollection =
           new Dictionary<string, AccessTokenBag>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 注册应用凭证信息，此操作只是注册，不会马上获取Token，并将清空之前的Token，
        /// </summary>
        /// <param name="corpId"></param>
        /// <param name="corpSecret"></param>
        public static void Register(string corpId, string corpSecret)
        {
            AccessTokenCollection[corpId + "_" + corpSecret] = new AccessTokenBag()
            {
                CorpId = corpId,
                CorpSecret = corpSecret,
                ExpireTime = DateTime.MinValue,
                TokenResult = new AccessTokenResult()
            };
        }

        /// <summary>
        /// 使用完整的应用凭证获取Token，如果不存在将自动注册
        /// </summary>
        /// <param name="corpId"></param>
        /// <param name="corpSecret"></param>
        /// <param name="getNewToken"></param>
        /// <returns></returns>
        public static string TryGetToken(string corpId, string corpSecret, bool getNewToken = false)
        {
            if (!CheckRegistered(corpId + "_" + corpSecret) || getNewToken)
            {
                Register(corpId, corpSecret);
            }
            return GetToken(corpId, corpSecret);
        }

        /// <summary>
        /// 获取可用Token
        /// </summary>
        /// <param name="corpId"></param>
        /// <param name="getNewToken">是否强制重新获取新的Token</param>
        /// <returns></returns>
        public static string GetToken(string corpId, string corpSecret, bool getNewToken = false)
        {
            return GetTokenResult(corpId,corpSecret, getNewToken).access_token;
        }

        /// <summary>
        /// 获取可用Token
        /// </summary>
        /// <param name="corpId"></param>
        /// <param name="getNewToken">是否强制重新获取新的Token</param>
        /// <returns></returns>
        public static AccessTokenResult GetTokenResult(string corpId, string corpSecret, bool getNewToken = false)
        {


           // return DLYB.Weixin.CommonAPIs.AccessTokenContainer.GetAccessTokenResult(corpId, corpSecret, CommonApi.GetToken, getNewToken);

            //if (!AccessTokenCollection.ContainsKey(corpId))
            //{
            //    throw new WeixinException("此CorpId尚未注册，请先使用AccessTokenContainer.Register完成注册（全局执行一次即可）！");
            //}

            if (!CheckRegistered(corpId + "_" + corpSecret))
            {
                Register(corpId, corpSecret);
            }

            var accessTokenBag = AccessTokenCollection[corpId + "_" + corpSecret];
            lock (accessTokenBag.Lock)
            {
                if (getNewToken || accessTokenBag.ExpireTime <= DateTime.Now)
                {
                    //已过期，重新获取
                    accessTokenBag.TokenResult = CommonApi.GetToken(accessTokenBag.CorpId,
                        accessTokenBag.CorpSecret);
                    accessTokenBag.ExpireTime = DateTime.Now.AddSeconds(7200);
                }
            }
            return accessTokenBag.TokenResult;
        }

        /// <summary>
        /// 检查是否已经注册
        /// </summary>
        /// <param name="corpId"></param>
        /// <returns></returns>
        public static bool CheckRegistered(string corpId)
        {
            return AccessTokenCollection.ContainsKey(corpId);
        }
    }
}
