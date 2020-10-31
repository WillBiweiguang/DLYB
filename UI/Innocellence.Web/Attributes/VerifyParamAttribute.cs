using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Service;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Innocellence.Web.Attributes
{
    public class VerifyParamAttribute : ActionFilterAttribute
    {
        public ILogger Logger { get; set; }
        public string Param { get; set; }


        public VerifyParamAttribute()
        {
            Logger = LogManager.GetLogger(this.GetType());
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var attributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(VerifyParamAttribute), true);

            if (attributes.Length < 1)
            {
                return;
            }

            VerifyParamAttribute attribute = attributes[0] as VerifyParamAttribute;

            if (!ApiCommon.CompareSignature(attribute.Param, filterContext.RequestContext.HttpContext.Request))
            {
                filterContext.Result = new JsonResult()
                {
                    Data = new { Status = "101", Message = "签名错误" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                return;
            }

            return;
        }

        private bool VerifyParam(string param, HttpRequestBase request)
        {

            //获取所有的参数
            param = param + ",appId,timestamp,nonce";

            Dictionary<string, string> dict = new Dictionary<string, string>();
            var _params = param.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var a in _params)
            {
                if (a == "url")
                {
                    dict.Add(a, request[a].Split(',')[0]);
                }
                else
                {
                    dict.Add(a, request[a]);
                }
            }
            string appid = dict["appId"];
            string appSignKey = CommonService.GetSysConfig(appid, "iMih0xabKQdw8CBbkTM5Ley84WhN4oL6u5lbDui6G9tUlQo7fJE1CcktZ2UiETnU1FZ0R3ZvzYLKOzmaziyms5QuMia8czkEwFv2TQUg4G45Ha0aHPEHXnhjVqUPnKPJ");
            dict.Add("appSignKey", appSignKey);
            var sign = ApiCommon.GetSignature(dict, request.Params);

            if (request["sign"] == null)
            {
                Logger.Error("sign error!this: {0}  \r\n Remote:{1}", sign, "null");
                return false;
            }
            else if (request["sign"].ToLower(CultureInfo.InvariantCulture) != sign)
            {
                Logger.Error("sign error!this: {0}  \r\n Remote:{1}", sign, request["sign"].ToLower(CultureInfo.InvariantCulture));
                return false;
            }
            else
            {
                Logger.Info("sign success!");
                return true;
            }
        }
    }

    public static class ApiCommon
    {
        private readonly static ILogger Logger = LogManager.GetLogger(typeof(ApiCommon));
        public static string GetSignature(Dictionary<string, string> paramDict, NameValueCollection Request)
        {
            StringBuilder sb = new StringBuilder();

            OrdinalComparer comp = new OrdinalComparer();

            //参数排序
            var keys = paramDict.Keys.OrderBy(a => a, comp);

            foreach (var a in keys)
            {
                sb.AppendFormat("&{0}={1}", a, paramDict[a]);
            }

            Logger.Info("GetSha1 Key:" + sb.ToString());

            //加密
            return GetSha1(sb.ToString().Substring(1)).ToLowerInvariant();
        }

        public static string GetSignature(string param, HttpRequestBase request)
        {
            param = param + ",appId,timestamp,nonce";

            Dictionary<string, string> dict = new Dictionary<string, string>();
            var _params = param.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var a in _params)
            {
                if (a == "url")
                {
                    dict.Add(a, request[a].Split(',')[0]);
                }
                else
                {
                    dict.Add(a, request[a]);
                }
            }
            string appid = dict["appId"];
            string appSignKey = CommonService.GetSysConfig(appid, "iMih0xabKQdw8CBbkTM5Ley84WhN4oL6u5lbDui6G9tUlQo7fJE1CcktZ2UiETnU1FZ0R3ZvzYLKOzmaziyms5QuMia8czkEwFv2TQUg4G45Ha0aHPEHXnhjVqUPnKPJ");
            dict.Add("appSignKey", appSignKey);
            return ApiCommon.GetSignature(dict, request.Params);
        }

        public static bool CompareSignature(string param, HttpRequestBase request)
        {
            param = param + ",appId,timestamp,nonce";

            Dictionary<string, string> dict = new Dictionary<string, string>();
            var _params = param.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var a in _params)
            {
                if (a == "url")
                {
                    dict.Add(a, request[a].Split(',')[0]);
                }
                else
                {
                    dict.Add(a, request[a]);
                }
            }
            string appid = dict["appId"];
            string appSignKey = CommonService.GetSysConfig(appid, "iMih0xabKQdw8CBbkTM5Ley84WhN4oL6u5lbDui6G9tUlQo7fJE1CcktZ2UiETnU1FZ0R3ZvzYLKOzmaziyms5QuMia8czkEwFv2TQUg4G45Ha0aHPEHXnhjVqUPnKPJ");
            dict.Add("appSignKey", appSignKey);
            var sign = ApiCommon.GetSignature(dict, request.Params);

            if (request["sign"] == null)
            {
                Logger.Error("sign error!this: {0}  \r\n Remote:{1}", sign, "null");
                return false;
            }
            else if (request["sign"].ToLower(CultureInfo.InvariantCulture) != sign)
            {
                Logger.Error("sign error!this: {0}  \r\n Remote:{1}", sign, request["sign"].ToLower(CultureInfo.InvariantCulture));
                return false;
            }
            else
            {
                Logger.Info("sign success!");
                return true;
            }
        }
        /// <summary>
        /// 公用的那个有bug，不能处理中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetSha1(string str)
        {
            //建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();
            //将mystr转换成byte[] 
            byte[] dataToHash = Encoding.UTF8.GetBytes(str);
            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);
            //将运算结果转换成string
            string hash = BitConverter.ToString(dataHashed).Replace("-", "");
            return hash;
        }
    }

    public class OrdinalComparer : System.Collections.Generic.IComparer<String>
    {
        public int Compare(String x, String y)
        {
            return string.CompareOrdinal(x, y);
        }
    }
}