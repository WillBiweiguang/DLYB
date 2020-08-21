using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Core.Logging;
using DLYB.CA.Entity;

namespace DLYB.CA.Service.Common
{
    public class WebRequestPost
    {
        private static string UserBehaviorUrl =CommonService.GetSysConfig("SSO Server", "") +
             System.Configuration.ConfigurationManager.AppSettings["UserBehaviorUrl"];
        private readonly ILogger _log = LogManager.GetLogger(typeof(UserBehavior));

        public static string PostToUserBehavior(string oid, string actionUrl, UInt64 moduleId)
        {
            return Post(UserBehaviorUrl, string.Format("oid={0}&actionId={1}&moduleId={2}", oid, actionUrl, moduleId), Encoding.UTF8);
        }

        public static string Post(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length); //写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {

            }
            return ret;
        }

        public void CallUserBehavior(string functionid, string userid, string Appid, string content, string url, int contenttype)
        {
            // Task.Factory.StartNew(() => Call(functionid, userid, Appid, content, url, contenttype));
            Call(functionid, userid, Appid, content, url, contenttype);
        }

        private void Call(string functionid, string userid, string Appid, string content, string url, int contentType)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {

                    var builder = new UriBuilder(new Uri(CommonService.GetSysConfig("SSO Server", ""))) { Path = System.Configuration.ConfigurationManager.AppSettings["UserBehaviorUrl"] };
                    _log.Debug("behaviorurl is:{0}", builder);
                    //var builder = new UriBuilder(UserBehaviorUrl);
                    var request = (HttpWebRequest)WebRequest.Create(builder.ToString());
                    var body = string.Format("functionId={0}&userid={1}&Appid={2}&Content={3}&url={4}&contentType={5}", functionid, userid, Appid, content, url, contentType);
                    _log.Debug("body is:{0}", body);
                    var byteArray = Encoding.UTF8.GetBytes(body);
                    _log.Debug("userbehavior url:{0}", url);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;
                    request.Accept = "*/*;application/json";

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(byteArray, 0, byteArray.Length);
                    }
                

                    var webResponse = (HttpWebResponse)request.GetResponse();
                    string responseJson;
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null)
                        {
                            throw new Exception("the response stream is null.");
                        }

                        using (var reader = new StreamReader(responseStream))
                        {
                            responseJson = reader.ReadToEnd();
                        }
                    }

                    Infrastructure.Core.Logging.LogManager.GetLogger(this.GetType()).Debug<string>("responseJson:" + responseJson);
                }
                catch (Exception ex)
                {
                    Infrastructure.Core.Logging.LogManager.GetLogger(this.GetType()).Error<string>("Behavior: " + ex.Message);
                    Infrastructure.Core.Logging.LogManager.GetLogger(this.GetType()).Error<string>("Trace: " + ex.StackTrace);
                }
            });

        }

    }
}
