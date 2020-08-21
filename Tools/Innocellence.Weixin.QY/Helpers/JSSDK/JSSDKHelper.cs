/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：JSSDKHelper.cs
    文件功能描述：JSSDK生成签名的方法等
    
    
    创建标识：DLYB - 20150313
    
    修改标识：DLYB - 20150313
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Text;

namespace DLYB.Weixin.QY.Helpers
{
    public class JSSDKHelper
    {
        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GetNoncestr()
        {
            Random random = new Random();
            return MD5UtilHelper.GetMD5(random.Next(1000).ToString(), "GBK");
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 获取JS-SDK权限验证的签名Signature
        /// </summary>
        /// <param name="jsapi_ticket">jsapi_ticket</param>
        /// <param name="noncestr">随机字符串(必须与wx.config中的nonceStr相同)</param>
        /// <param name="timestamp">时间戳(必须与wx.config中的timestamp相同)</param>
        /// <param name="url">当前网页的URL，不包含#及其后面部分(必须是调用JS接口页面的完整URL)</param>
        /// <returns></returns>
        public static string GetSignature(string jsapi_ticket, string noncestr, string timestamp, string url)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("jsapi_ticket=").Append(jsapi_ticket).Append("&")
             .Append("noncestr=").Append(noncestr).Append("&")
             .Append("timestamp=").Append(timestamp).Append("&")
             .Append("url=").Append(url.IndexOf("#") >= 0 ? url.Substring(0, url.IndexOf("#")) : url);
            return SHA1UtilHelper.GetSha1(sb.ToString()).ToLower();
        }
   
        /// <summary>
        /// 获取给UI使用的JSSDK信息包
        /// </summary>
        /// <param name="corpId"></param>
        /// <param name="corpSecret"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static JsSdkUiPackage GetJsSdkUiPackage(string corpId, string corpSecret, string url)
        {
            //获取时间戳
            var timestamp = GetTimestamp();
            //获取随机码
            string nonceStr = GetNoncestr();
            string ticket = DLYB.Weixin.QY.CommonAPIs.JsApiTicketContainer.TryGetTicket(corpId, corpSecret);
            //获取签名
            string signature = JSSDKHelper.GetSignature(ticket, nonceStr, timestamp, url);
            //返回信息包
            return new JsSdkUiPackage(corpId, timestamp, nonceStr, signature);
        }

    }
}
