/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：AccessTokenContainer.cs
    文件功能描述：通用接口AccessToken容器，用于自动管理AccessToken，如果过期会重新获取
    
    
    创建标识：Innocellence - 20150211
    
    修改标识：Innocellence - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innocellence.Weixin.Exceptions;
using Innocellence.Weixin.Entities;
//using Infrastructure.Core;
//using Innocellence.Weixin.Entity;

namespace Innocellence.Weixin.CommonAPIs
{


    
    /// <summary>
    /// 通用接口AccessToken容器，用于自动管理AccessToken，如果过期会重新获取
    /// </summary>
    public class AccessTokenDBContainer 
    {

        public static object Lock = new object();
       // public static IRepository<SysWechatConfig, int> RepConfig = null;

        public static Func<string, string, object> FindTokenFromDB = null;

        public static Func<object, int> UpdateTokenToDB = null;

        static Dictionary<string, AccessTokenBag> AccessTokenCollection =
           new Dictionary<string, AccessTokenBag>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 注册应用凭证信息，此操作只是注册，不会马上获取Token，并将清空之前的Token，
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        public static void Register(string appId, string appSecret) 
        {
            AccessTokenCollection[appId] = new AccessTokenBag()
            {
                CorpId = appId, 
                 CorpSecret = appSecret,
                ExpireTime = DateTime.MinValue,
                TokenResult = new AccessTokenResult()
            };
        }

        /// <summary>
        /// 使用完整的应用凭证获取Token，如果不存在将自动注册
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="getNewToken"></param>
        /// <returns></returns>
        public static string TryGetToken(string appId, string appSecret, Func<string, string, string, AccessTokenResult> func, bool getNewToken = false)
        {
            //lock (Lock)
            //{
            //    if (!CheckRegistered(appId) || getNewToken)
            //    {
            //        Register(appId, appSecret);
            //    }

            //}
            
            return GetToken(appId,appSecret, func);
        }

        /// <summary>
        /// 获取可用Token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="getNewToken">是否强制重新获取新的Token</param>
        /// <returns></returns>
        public static string GetToken(string appId, string appSecret, Func<string, string, string, AccessTokenResult> func, bool getNewToken = false)
        {
            return GetTokenResult(appId, appSecret,func, getNewToken).access_token;
        }

        /// <summary>
        /// 获取可用Token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="getNewToken">是否强制重新获取新的Token</param>
        /// <returns></returns>
        public static AccessTokenResult GetTokenResult(string appId, string appSecret, Func<string, string, string, AccessTokenResult> func, bool getNewToken = false)
        {

            if (FindTokenFromDB == null || UpdateTokenToDB==null)
            {
                throw new WeixinException("微信关联的DB对象未初始化！");
            }

            dynamic objConfig = FindTokenFromDB(appId, appSecret);// RepConfig.Entities.Where(a => a.WeixinCorpId == appId && a.IsDeleted == false).FirstOrDefault();

            if (objConfig==null)
            {
                throw new WeixinException("此appId尚未注册，请先在数据库中注册！");
            }

          //  var accessTokenBag = AccessTokenCollection[appId];
            lock (Lock)
            {
                if (getNewToken ||!((DateTime?)objConfig.AccessTokenExpireTime).HasValue|| objConfig.AccessTokenExpireTime <= DateTime.Now)
                {
                    //已过期，重新获取
                   var Result = func(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret,"");// CommonApi.GetToken(accessTokenBag.AppId, accessTokenBag.AppSecret);
                   var ExpireTime = DateTime.Now.AddSeconds((Result.expires_in == 0 ? 7200 : Result.expires_in));

                   //SysWechatConfig objNew = new SysWechatConfig() 
                   //{ 
                   //    Id=objConfig.Id,
                   //    AccessToken = Result.access_token,
                   //    AccessTokenExpireTime = ExpireTime
                   //};


                 
                   objConfig.AccessToken = Result.access_token;
                   objConfig.AccessTokenExpireTime = ExpireTime;

                   UpdateTokenToDB(objConfig);

                }
            }


            return new AccessTokenResult { access_token = objConfig.AccessToken };
        }

        /// <summary>
        /// 检查是否已经注册
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckRegistered(string appId)
        {
            return true;// AccessTokenCollection.ContainsKey(appId);
        }
    }
}
