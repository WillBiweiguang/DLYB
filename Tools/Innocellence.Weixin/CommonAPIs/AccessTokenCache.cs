/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB

    文件名：AccessTokenContainer.cs
    文件功能描述：通用接口AccessToken容器，用于自动管理AccessToken，如果过期会重新获取


    创建标识：DLYB - 20150211

    修改标识：DLYB - 20150303
    修改描述：整理接口

    修改标识：DLYB - 20150702
    修改描述：添加GetFirstOrDefaultAppId()方法

    修改标识：DLYB - 20151004
    修改描述：v13.3.0 将JsApiTicketContainer整合到AccessTokenContainer

    修改标识：DLYB - 20160318
    修改描述：13.6.10 使用FlushCache.CreateInstance使注册过程立即生效

----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using DLYB.Weixin.Containers;
using DLYB.Weixin.Exceptions;
using DLYB.Weixin.CacheUtility;
using DLYB.Weixin.Entities;
using DLYB.Weixin.Cache;


namespace DLYB.Weixin.CommonAPIs
{


    /// <summary>
    /// 通用接口AccessToken容器，用于自动管理AccessToken，如果过期会重新获取
    /// </summary>
    public class AccessTokenCache<T, T1> : BaseContainer<T>
        where T : BaseContainerBag<T1>, new()
        where T1 : BaseTokenResult, new()
    {

        //private static IBaseCacheStrategy<string, AccessTokenBag> /*IBaseCacheStrategy<string,Dictionary<string, TBag>>*/ Cache
        //{
        //    get
        //    {
        //        //使用工厂模式或者配置进行动态加载
        //        return CacheStrategyFactory.GetContainerCacheStragegyInstance();
        //    }
        //}

        private static object Lock = new object();

        ///// <summary>
        ///// 注册应用凭证信息，此操作只是注册，不会马上获取Token，并将清空之前的Token
        ///// </summary>
        ///// <param name="appId">微信公众号后台的【开发】>【基本配置】中的“AppID(应用ID)”</param>
        ///// <param name="appSecret">微信公众号后台的【开发】>【基本配置】中的“AppSecret(应用密钥)”</param>
        //public static void Register(string appId, string appSecret)
        //{
        //    using (FlushCache.CreateInstance())
        //    {
        //        Update(appId + "_" + appSecret, new AccessTokenBag()
        //        {
        //            AppId = appId,
        //            AppSecret = appSecret,
        //            AccessTokenExpireTime = DateTime.MinValue,
        //            AccessTokenResult = new AccessTokenResult()
        //        });
        //    }

        //    //为JsApiTicketContainer进行自动注册
        //   // JsApiTicketContainer.Register(appId, appSecret);
        //}

        /// <summary>
        /// 返回已经注册的第一个AppId
        /// </summary>
        /// <returns></returns>
        public static string GetFirstOrDefaultAppId()
        {
           // return ItemCollection.GetAll().Keys.FirstOrDefault();
            return null;
        }

        #region AccessToken

        ///// <summary>
        ///// 使用完整的应用凭证获取Token，如果不存在将自动注册
        ///// </summary>
        ///// <param name="appId"></param>
        ///// <param name="appSecret"></param>
        ///// <param name="getNewToken"></param>
        ///// <returns></returns>
        //public static string TryGetAccessToken(string appId, string appSecret, Func<string, string, string, AccessTokenResult> func, bool getNewToken = false)
        //{
        //    if (!CheckRegistered(appId + "_" + appSecret) || getNewToken)
        //    {
        //        Register(appId, appSecret);
        //    }
        //    return GetAccessToken(appId,appSecret,func);
        //}

        ///// <summary>
        ///// 获取可用Token
        ///// </summary>
        ///// <param name="appId"></param>
        ///// <param name="getNewToken">是否强制重新获取新的Token</param>
        ///// <returns></returns>
        //public static string GetAccessToken(string appId, string appSecret, Func<string, string, string, T1> func, bool getNewToken = false)
        //{
        //    return GetAccessTokenResult(appId, appSecret, func,getNewToken).TokenResult.access_token;
        //}

        /// <summary>
        /// 获取可用AccessTokenResult对象
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="getNewToken">是否强制重新获取新的Token</param>
        /// <returns></returns>
        public static T GetAccessTokenResult(string appId, string appSecret, Func<string, string, string, T1> func, bool getNewToken = false)
        {
            //if (!CheckRegistered(appId + "_" + appSecret))
            //{
            //    Register(appId, appSecret);
            //   // throw new UnRegisterAppIdException(appId, string.Format("此appId（{0}）尚未注册，请先使用AccessTokenContainer.Register完成注册（全局执行一次即可）！", appId));
            //}

            string strKey = appId + "_" + appSecret+"_"+ typeof(T).Name;

            var accessTokenBag =getNewToken?default(T): (T)GetItem(strKey);
            //lock (accessTokenBag.Lock)
            lock (Lock)
            {
                if (accessTokenBag == null)
                {
                  //  Register(appId, appSecret);
                  

                    accessTokenBag =new T()
                   {
                       CorpId = appId, 
                       CorpSecret = appSecret,
                       ExpireTime = DateTime.MinValue,
                       TokenResult = new T1()
                   };
                }

                if (getNewToken || accessTokenBag.ExpireTime <= DateTime.Now)
                {
                    //已过期，重新获取
                    accessTokenBag.TokenResult = func(accessTokenBag.CorpId, accessTokenBag.CorpSecret, "client_credential");
                    accessTokenBag.ExpireTime = DateTime.Now.AddSeconds(accessTokenBag.TokenResult.expires_in).AddMinutes(-5);

                    Update(strKey, accessTokenBag); //更新缓存
                }

               
            }

           
            return accessTokenBag;
        }


        #endregion

    }
}
