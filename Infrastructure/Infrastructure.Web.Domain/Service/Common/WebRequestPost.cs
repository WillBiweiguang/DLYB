using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Web.Domain.Service.Common
{
    public class WebRequestPost
    {
        static string UserBehaviorUrl = System.Configuration.ConfigurationManager.AppSettings["UserBehaviorUrl"];

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
    }
}
