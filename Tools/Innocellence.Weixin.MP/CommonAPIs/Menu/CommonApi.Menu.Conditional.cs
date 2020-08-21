/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：CommonApi.Menu.Conditional
    文件功能描述：个性化自定义菜单接口
    
    
    创建标识：DLYB - 20151222
        
    修改标识：DLYB - 20151222
    修改描述：v13.5.1 添加个性化菜单接口
----------------------------------------------------------------*/

/*
    API：http://mp.weixin.qq.com/wiki/0/c48ccd12b69ae023159b4bfaa7c39c20.html
 */

using DLYB.Weixin.CommonAPIs;
using DLYB.Weixin.Entities;
using DLYB.Weixin.Helpers;
using DLYB.Weixin.HttpUtility;
using DLYB.Weixin.MP.Entities;
using DLYB.Weixin.MP.Entities.Menu;

namespace DLYB.Weixin.MP.CommonAPIs
{
    public partial class CommonApi
    {
        /// <summary>
        /// 创建个新华菜单
        /// </summary>
        /// <param name="AppId">AccessToken或AppId。当为AppId时，如果AccessToken错误将自动获取一次。当为null时，获取当前注册的第一个AppId。</param>
        /// <param name="buttonData">菜单内容</param>
        /// <returns></returns>
        public static CreateMenuConditionalResult CreateMenuConditional(string AppId, string AppSecret, ConditionalButtonGroup buttonData, int timeOut = Config.TIME_OUT)
        {
            return ApiHandlerWapper.TryCommonApi(accessToken =>
             {
                 var urlFormat = "https://api.weixin.qq.com/cgi-bin/menu/addconditional?access_token={0}";
                 var jsonSetting = new JsonSetting(true);
                 return CommonJsonSend.Send<CreateMenuConditionalResult>(accessToken, urlFormat, buttonData, timeOut: timeOut, jsonSetting: jsonSetting);

             }, AppId, AppSecret);
        }


        #region GetMenu

        /* 使用普通自定义菜单查询接口可以获取默认菜单和全部个性化菜单信息，请见自定义菜单查询接口的说明 */

        /// <summary>
        /// 测试个性化菜单匹配结果
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="userId">可以是粉丝的OpenID，也可以是粉丝的微信号。</param>
        /// <returns></returns>
        public static MenuTryMatchResult TryMatch(string AppId, string AppSecret, string userId)
        {
            return ApiHandlerWapper.TryCommonApi(accessToken =>
            {
                var url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/delconditional?access_token={0}", accessToken.AsUrlData());

                var data = new
                {
                    user_id = userId
                };

                return CommonJsonSend.Send<MenuTryMatchResult>(accessToken, url, data, CommonJsonSendType.POST);

            }, AppId, AppSecret);
        }

        #endregion

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="menuId">菜单Id</param>
        /// <returns></returns>
        public static WxJsonResult DeleteMenuConditional(string AppId, string AppSecret, string menuId)
        {
            return ApiHandlerWapper.TryCommonApi(accessToken =>
            {
                var url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/delconditional?access_token={0}", accessToken.AsUrlData());

                var data = new
                {
                    menuId = menuId
                };

                return CommonJsonSend.Send(accessToken, url, data, CommonJsonSendType.POST);

            }, AppId, AppSecret);

        }

        /* 使用普通自定义菜单删除接口可以删除所有自定义菜单（包括默认菜单和全部个性化菜单），请见自定义菜单删除接口的说明。 */
    }
}