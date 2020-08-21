// -----------------------------------------------------------------------
//  <copyright file="UserAgentHelper.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 18:48</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace Infrastructure.Utility.Web
{
    /// <summary>
    /// UserAgent辅助操作类
    /// </summary>
    public static class WebRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static string doRequestHttpClient(string strUrl, List<KeyValuePair<String, String>> paramList)
        {
            HttpClientHandler handler = new HttpClientHandler();

            HttpClient httpclient = new HttpClient(handler);
            httpclient.DefaultRequestHeaders.Add("Accept", "*/*");
            //  httpclient.DefaultRequestHeaders.Add("Referer", strUrl);
            // httpclient.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
            httpclient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            var response = httpclient.PostAsync(new Uri(strUrl), new FormUrlEncodedContent(paramList)).Result;
            return response.Content.ReadAsStringAsync().Result;



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="strEncoding"></param>
        /// <returns></returns>
        public static string GetHtmlFromUrl(string Url, string strEncoding = "UTF-8")
        {

            System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
            // Get the response instance.
            System.Net.WebResponse wResp = wReq.GetResponse();
            System.IO.Stream respStream = wResp.GetResponseStream();
            // Dim reader As StreamReader = New StreamReader(respStream)
            using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(strEncoding)))
            {
                return reader.ReadToEnd();
            }
        }
    }
}