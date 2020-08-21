/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：Post.cs
    文件功能描述：Post
    
    
    创建标识：DLYB - 20150211
    
    修改标识：DLYB - 20150303
    修改描述：整理接口
 
    修改标识：DLYB - 20150312
    修改描述：开放代理请求超时时间
 
    修改标识：zhanghao-kooboo - 20150316
    修改描述：增加
 
    修改标识：DLYB - 20150407
    修改描述：发起Post请求方法修改，为了上传永久视频素材
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using DLYB.Weixin.Entities;
using DLYB.Weixin.Exceptions;
using DLYB.Weixin.Logging;

namespace DLYB.Weixin.HttpUtility
{
    public static class Post
    {
        /// <summary>
        /// 获取Post结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="returnText"></param>
        /// <returns></returns>
        public static T GetResult<T>(string returnText)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();

            if (returnText.Contains("errcode"))
            {
                //可能发生错误
                WxJsonResult errorResult = js.Deserialize<WxJsonResult>(returnText);
                if (errorResult.errcode != ReturnCode.请求成功)
                {

                    Log4NetLog.Logger.ErrorFormat("微信请求发生错误！错误代码：{0}，说明：{1}",
                                      (int)errorResult.errcode,
                                      errorResult.errmsg);

                    //发生错误
                    throw new ErrorJsonResultException(
                        string.Format("微信Post请求发生错误！错误代码：{0}，说明：{1}",
                                      (int)errorResult.errcode,
                                      errorResult.errmsg),
                        null, errorResult);
                }
            }

            T result = js.Deserialize<T>(returnText);
            return result;
        }


        #region 同步方法


        /// <summary>
        /// 发起Post请求
        /// </summary>
        /// <typeparam name="T">返回数据类型（Json对应的实体）</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="cookieContainer">CookieContainer，如果不需要则设为null</param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <returns></returns>
        public static T PostFileGetJson<T>(string url, CookieContainer cookieContainer = null,
            Dictionary<string, Stream> fileDictionary = null,
            Dictionary<string, string> postDataDictionary = null,
            Encoding encoding = null,
            int timeOut = Config.TIME_OUT)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                postDataDictionary.FillFormDataStream(ms); //填充formData
                string returnText = HttpUtility.RequestUtility.HttpPost(url, cookieContainer, ms, fileDictionary, null, encoding, timeOut: timeOut);
                var result = GetResult<T>(returnText);
                return result;
            }

        }

        /// <summary>
        /// 发起Post请求
        /// </summary>
        /// <typeparam name="T">返回数据类型（Json对应的实体）</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="cookieContainer">CookieContainer，如果不需要则设为null</param>
        /// <param name="encoding"></param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <param name="fileDictionary"></param>
        /// <param name="postDataDictionary"></param>
        /// <returns></returns>
        public static T PostFileGetJson<T>(string url, CookieContainer cookieContainer = null, Dictionary<string, string> fileDictionary = null, Dictionary<string, string> postDataDictionary = null, Encoding encoding = null, int timeOut = Config.TIME_OUT)
        {

            Dictionary<string, Stream> fileDictionary_ = new Dictionary<string, Stream>();

            foreach(var a in fileDictionary){
                 fileDictionary_.Add(a.Value,null);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                postDataDictionary.FillFormDataStream(ms); //填充formData
                string returnText = RequestUtility.HttpPost(url, cookieContainer, ms, fileDictionary_, null, encoding, timeOut: timeOut);
                var result = GetResult<T>(returnText);
                return result;
            }
        }

        /// <summary>
        /// 发起Post请求
        /// </summary>
        /// <typeparam name="T">返回数据类型（Json对应的实体）</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="cookieContainer">CookieContainer，如果不需要则设为null</param>
        /// <param name="fileStream">文件流</param>
        /// <param name="encoding"></param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <param name="checkValidationResult">验证服务器证书回调自动验证</param>
        /// <returns></returns>
        public static T PostGetJson<T>(string url, CookieContainer cookieContainer = null, Stream fileStream = null, Encoding encoding = null, int timeOut = Config.TIME_OUT, bool checkValidationResult = false)
        {
           // string returnText = RequestUtility.HttpPost(url, cookieContainer, fileStream, null, null, encoding, timeOut: timeOut, checkValidationResult: checkValidationResult);
            string returnText = RequestUtility.HttpPost(url, cookieContainer, fileStream, null, null, encoding, timeOut: timeOut);
         //   WeixinTrace.SendLog(url, returnText);

            Log4NetLog.Logger.DebugFormat("PostGetJson url:{0} Result:{1}", url, returnText);

            var result = GetResult<T>(returnText);
            return result;
        }

        public static T PostGetJson<T>(string url, CookieContainer cookieContainer , string strJson , Encoding encoding = null, int timeOut = Config.TIME_OUT, bool checkValidationResult = false)
        {


            using (MemoryStream ms = new MemoryStream())
            {
                var bytes = Encoding.UTF8.GetBytes(strJson);
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0, SeekOrigin.Begin);

                return Post.PostGetJson<T>(url, cookieContainer, ms,encoding, timeOut, checkValidationResult);
            }

            // string returnText = RequestUtility.HttpPost(url, cookieContainer, fileStream, null, null, encoding, timeOut: timeOut, checkValidationResult: checkValidationResult);
        //    string returnText = RequestUtility.HttpPost(url, cookieContainer, fileStream, null, null, encoding, timeOut: timeOut);
            //   WeixinTrace.SendLog(url, returnText);

          //  Log4NetLog.Logger.DebugFormat("PostGetJson url:{0} Result:{1}", url, returnText);

          //  var result = GetResult<T>(returnText);
          //  return result;
        }

        /// <summary>
        /// PostGetJson
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="formData"></param>
        /// <param name="encoding"></param>
        /// <param name="timeOut"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T PostGetJson<T>(string url, CookieContainer cookieContainer = null, Dictionary<string, string> formData = null, Encoding encoding = null, int timeOut = Config.TIME_OUT)
        {
            string returnText = RequestUtility.HttpPost(url, cookieContainer, formData, encoding, timeOut: timeOut);
            var result = GetResult<T>(returnText);
            return result;
        }

        /// <summary>
        /// 使用Post方法上传数据并下载文件或结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="stream"></param>
        public static void Download(string url, string data, Stream stream)
        {
            WebClient wc = new WebClient();
            var file = wc.UploadData(url, "POST", Encoding.UTF8.GetBytes(string.IsNullOrEmpty(data) ? "" : data));
            foreach (var b in file)
            {
                stream.WriteByte(b);
            }
        }

        #endregion

        #region 异步方法

        /// <summary>
        /// PostFileGetJson的异步版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="fileDictionary"></param>
        /// <param name="encoding"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static async Task<T> PostFileGetJsonAsync<T>(string url, CookieContainer cookieContainer = null, Dictionary<string, string> fileDictionary = null, Encoding encoding = null, int timeOut = Config.TIME_OUT)
        {
            string returnText = await RequestUtility.HttpPostAsync(url, cookieContainer, null, fileDictionary, null, encoding, timeOut);
            var result = GetResult<T>(returnText);
            return result;
        }

        /// <summary>
        /// PostGetJson的异步版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="fileStream"></param>
        /// <param name="encoding"></param>
        /// <param name="timeOut"></param>
        /// <param name="checkValidationResult"></param>
        /// <returns></returns>
        public static async Task<T> PostGetJsonAsync<T>(string url, CookieContainer cookieContainer = null, Stream fileStream = null, Encoding encoding = null, int timeOut = Config.TIME_OUT, bool checkValidationResult = false)
        {
            string returnText = await RequestUtility.HttpPostAsync(url, cookieContainer, fileStream, null, null, encoding, timeOut, checkValidationResult: checkValidationResult);
            var result = GetResult<T>(returnText);
            return result;
        }

        /// <summary>
        /// PostGetJson的异步版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="formData"></param>
        /// <param name="encoding"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static async Task<T> PostGetJsonAsync<T>(string url, CookieContainer cookieContainer = null, Dictionary<string, string> formData = null, Encoding encoding = null, int timeOut = Config.TIME_OUT)
        {
            string returnText = await RequestUtility.HttpPostAsync(url, cookieContainer, formData, encoding, timeOut);
            var result = GetResult<T>(returnText);
            return result;
        }

        /// <summary>
        /// 使用Post方法上传数据并下载文件或结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="stream"></param>
        public static async Task DownloadAsync(string url, string data, Stream stream)
        {
            WebClient wc = new WebClient();

            var fileBytes = await wc.UploadDataTaskAsync(url, "POST", Encoding.UTF8.GetBytes(string.IsNullOrEmpty(data) ? "" : data));
            await stream.WriteAsync(fileBytes, 0, fileBytes.Length);//也可以分段写入
        }

        #endregion
    }
}
