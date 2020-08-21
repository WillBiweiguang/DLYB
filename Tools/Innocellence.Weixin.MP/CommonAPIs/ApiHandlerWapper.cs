/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：ApiHandlerWapper.cs（v12之前原AccessTokenHandlerWapper.cs）
    文件功能描述：使用AccessToken进行操作时，如果遇到AccessToken错误的情况，重新获取AccessToken一次，并重试
    
    
    创建标识：DLYB - 20150211
    
    修改标识：DLYB - 20150303
    修改描述：整理接口
    
    修改标识：DLYB - 20150703
    修改描述：添加TryCommonApi()方法
----------------------------------------------------------------*/

using System;
using DLYB.Weixin.Entities;
using DLYB.Weixin.Exceptions;

using DLYB.Weixin.Utilities.WeixinUtility;
using DLYB.Weixin.CommonAPIs;

namespace DLYB.Weixin.MP
{
    /// <summary>
    /// 针对AccessToken无效或过期的自动处理类
    /// </summary>
    public static class ApiHandlerWapper
    {
        /// <summary>
        /// 使用AccessToken进行操作时，如果遇到AccessToken错误的情况，重新获取AccessToken一次，并重试。
        /// 使用此方法之前必须使用AccessTokenContainer.Register(_appId, _appSecret);或JsApiTicketContainer.Register(_appId, _appSecret);方法对账号信息进行过注册，否则会出错。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fun"></param>
        /// <param name="AppId">AccessToken或AppId。如果为null，则自动取已经注册的第一个appId/appSecret来信息获取AccessToken。</param>
        /// <param name="retryIfFaild">请保留默认值true，不用输入。</param>
        /// <returns></returns>
        public static T TryCommonApi<T>(Func<string, T> fun, string AppId ,string strSecret, bool retryIfFaild = true) where T : WxJsonResult
        {
            string appId = null;
            string accessToken = null;

            if (AppId == null)
            {
                //appId = AccessTokenContainer.GetFirstOrDefaultAppId();
                if (appId == null)
                {
                    throw new UnRegisterAppIdException(null,
                        "尚无已经注册的AppId，请先使用AccessTokenContainer.Register完成注册（全局执行一次即可）！");
                }
            }
            else if (ApiUtility.IsAppId(AppId))
            {
                //if (!AccessTokenContainer.CheckRegistered(AppId))
                //{
                //    throw new UnRegisterAppIdException(AppId, string.Format("此appId（{0}）尚未注册，请先使用AccessTokenContainer.Register完成注册（全局执行一次即可）！", AppId));
                //}

                appId = AppId;
            }
            else
            {
                //accessToken
                accessToken = AppId;
            }


            T result = null;

            try
            {
                if (accessToken == null)
                {
                    var accessTokenResult = AccessTokenContainer.GetAccessTokenResult(appId,strSecret,DLYB.Weixin.MP.CommonAPIs.CommonApi.GetToken, false);
                    accessToken = accessTokenResult.access_token;
                }
                result = fun(accessToken);
            }
            catch (ErrorJsonResultException ex)
            {
                if (retryIfFaild
                    && appId != null
                    && ex.JsonResult.errcode == ReturnCode.获取access_token时AppSecret错误或者access_token无效)
                {

                    AccessTokenContainer.UpdateNull(appId, strSecret);

                    //尝试重新验证
                    var accessTokenResult = AccessTokenContainer.GetAccessTokenResult(appId, strSecret, DLYB.Weixin.MP.CommonAPIs.CommonApi.GetToken, false);//强制获取并刷新最新的AccessToken

                    if (accessToken == accessTokenResult.access_token)
                    {
                      accessTokenResult=  AccessTokenContainer.GetAccessTokenResult(appId, strSecret, DLYB.Weixin.MP.CommonAPIs.CommonApi.GetToken, true);
                    }
                    

                    accessToken = accessTokenResult.access_token;


                    result = fun(accessToken);
                    //result = TryCommonApi(fun, appId,strSecret, false);
                }
                else
                {
                    throw;
                }
            }

            return result;
        }



        public static T TryCommonTicketApi<T>(Func<string, T> fun, string AppId, string strSecret, bool retryIfFaild = true) where T : WxJsonResult
        {
            string appId = null;
            string ticket = null;

            if (AppId == null)
            {
                //appId = AccessTokenContainer.GetFirstOrDefaultAppId();
                if (appId == null)
                {
                    throw new UnRegisterAppIdException(null,
                        "尚无已经注册的AppId，请先使用AccessTokenContainer.Register完成注册（全局执行一次即可）！");
                }
            }
            else if (ApiUtility.IsAppId(AppId))
            {
                //if (!AccessTokenContainer.CheckRegistered(AppId))
                //{
                //    throw new UnRegisterAppIdException(AppId, string.Format("此appId（{0}）尚未注册，请先使用AccessTokenContainer.Register完成注册（全局执行一次即可）！", AppId));
                //}

                appId = AppId;
            }
            else
            {
                //accessToken
                ticket = AppId;
            }


            T result = null;

            try
            {
                if (ticket == null)
                {
                    var accessTokenResult = JsApiTicketContainer.GetJsApiTicketResult(appId, strSecret, DLYB.Weixin.MP.CommonAPIs.CommonApi.GetTicket, false);
                    ticket = accessTokenResult.ticket;
                }
                result = fun(ticket);
            }
            catch (ErrorJsonResultException ex)
            {
                if (retryIfFaild
                    && appId != null
                    && ex.JsonResult.errcode == ReturnCode.Ticket不合法)
                {

                    JsApiTicketContainer.UpdateNull(appId, strSecret);

                    //尝试重新验证
                    var accessTokenResult = JsApiTicketContainer.GetJsApiTicketResult(appId, strSecret, DLYB.Weixin.MP.CommonAPIs.CommonApi.GetTicket, false);//强制获取并刷新最新的AccessToken

                    if (ticket == accessTokenResult.ticket)
                    {
                        accessTokenResult = JsApiTicketContainer.GetJsApiTicketResult(appId, strSecret, DLYB.Weixin.MP.CommonAPIs.CommonApi.GetTicket, true);
                    }


                    ticket = accessTokenResult.ticket;


                    result = fun(ticket);
                    //result = TryCommonApi(fun, appId,strSecret, false);
                }
                else
                {
                    throw;
                }
            }

            return result;
        }

        /// <summary>
        /// 使用AccessToken进行操作时，如果遇到AccessToken错误的情况，重新获取AccessToken一次，并重试
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="fun">第一个参数为accessToken</param>
        /// <param name="retryIfFaild"></param>
        /// <returns></returns>
        [Obsolete("请使用TryCommonApi()方法")]
        public static T Do<T>(Func<string, T> fun, string appId, string appSecret, bool retryIfFaild = true)
            where T : WxJsonResult
        {
            T result = null;
            try
            {
                var accessToken = AccessTokenContainer.GetToken(appId, appSecret, DLYB.Weixin.MP.CommonAPIs.CommonApi.GetToken, false);
                result = fun(accessToken);
            }
            catch (ErrorJsonResultException ex)
            {
                if (retryIfFaild && ex.JsonResult.errcode == ReturnCode.获取access_token时AppSecret错误或者access_token无效)
                {
                    //尝试重新验证
                    var accessToken = AccessTokenContainer.GetToken(appId, appSecret, DLYB.Weixin.MP.CommonAPIs.CommonApi.GetToken, true);
                    result = Do(fun, appId, appSecret, false);
                }
            }
            return result;
        }



        public static Func<string,string> GetAppSecret;

    }
}
