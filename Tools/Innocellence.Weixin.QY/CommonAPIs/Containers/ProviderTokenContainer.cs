/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB

    文件名：ProviderTokenContainer.cs
    文件功能描述：通用接口ProviderToken容器，用于自动管理ProviderToken，如果过期会重新获取


    创建标识：DLYB - 20150313

    修改标识：DLYB - 20150313
    修改描述：整理接口

    修改标识：DLYB - 20160206
    修改描述：将public object Lock更改为internal object Lock

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

namespace DLYB.Weixin.QY.CommonAPIs
{
    /// <summary>
    /// ProviderTokenBag
    /// </summary>
    [Serializable]
    public class ProviderTokenBag : BaseContainerBag<ProviderTokenResult>
    {
       
        /// <summary>
        /// ProviderTokenResult
        /// </summary>
        public ProviderTokenResult ProviderTokenResult
        {
            get;
            set;
        }

        /// <summary>
        /// 只针对这个CorpId的锁
        /// </summary>
        internal object Lock = new object();

        private string _corpId;
        private string _corpSecret;
        private DateTime _expireTime;
        private ProviderTokenResult _providerTokenResult;
    }

    /// <summary>
    /// 通用接口ProviderToken容器，用于自动管理ProviderToken，如果过期会重新获取
    /// </summary>
    public class ProviderTokenContainer : BaseContainer<ProviderTokenBag>
    {
        private const string UN_REGISTER_ALERT = "此CorpId尚未注册，ProviderTokenContainer.Register完成注册（全局执行一次即可）！";

        /// <summary>
        /// 注册应用凭证信息，此操作只是注册，不会马上获取Token，并将清空之前的Token，
        /// </summary>
        /// <param name="corpId"></param>
        /// <param name="corpSecret"></param>
        public static void Register(string corpId, string corpSecret)
        {
            using (FlushCache.CreateInstance())
            {
                Update(corpId, new ProviderTokenBag()
                {
                    CorpId = corpId,
                    CorpSecret = corpSecret,
                    ExpireTime = DateTime.MinValue,
                    ProviderTokenResult = new ProviderTokenResult()
                });
            }
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
            if (!CheckRegistered(corpId) || getNewToken)
            {
                Register(corpId, corpSecret);
            }
            return GetToken(corpId);
        }

        /// <summary>
        /// 获取可用Token
        /// </summary>
        /// <param name="corpId"></param>
        /// <param name="getNewToken">是否强制重新获取新的Token</param>
        /// <returns></returns>
        public static string GetToken(string corpId, bool getNewToken = false)
        {
            return GetTokenResult(corpId, getNewToken).provider_access_token;
        }

        /// <summary>
        /// 获取可用Token
        /// </summary>
        /// <param name="corpId"></param>
        /// <param name="getNewToken">是否强制重新获取新的Token</param>
        /// <returns></returns>
        public static ProviderTokenResult GetTokenResult(string corpId, bool getNewToken = false)
        {
            if (!CheckRegistered(corpId))
            {
                throw new WeixinQyException(UN_REGISTER_ALERT);
            }

            var providerTokenBag = (ProviderTokenBag)GetItem(corpId);
            lock (providerTokenBag.Lock)
            {
                if (getNewToken || providerTokenBag.ExpireTime <= DateTime.Now)
                {
                    //已过期，重新获取
                    providerTokenBag.ProviderTokenResult = CommonApi.GetProviderToken(providerTokenBag.CorpId,
                        providerTokenBag.CorpSecret);
                    providerTokenBag.ExpireTime = DateTime.Now.AddSeconds(providerTokenBag.ProviderTokenResult.expires_in);
                }
            }
            return providerTokenBag.ProviderTokenResult;
        }

        /// <summary>
        /// 检查是否已经注册
        /// </summary>
        /// <param name="corpId"></param>
        /// <returns></returns>
        public new static bool CheckRegistered(string corpId)
        {
            return true;// ItemCollection.CheckExisted(corpId);
        }
    }
}
