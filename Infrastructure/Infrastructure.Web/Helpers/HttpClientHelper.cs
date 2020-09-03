using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Web.Helpers
{
    public class HttpClientHelper
    {
        public static string HttpClientGet(string url, IList<KeyValuePair<string, string>> parameters, int httpClientTimeout = 10, bool isWithSign = false)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(httpClientTimeout);
                var content = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
                string symbol = isWithSign ? "&" : "?";
                var response = string.IsNullOrEmpty(content) ? httpClient.GetAsync(url).GetAwaiter().GetResult() : httpClient.GetAsync(url + symbol + content).GetAwaiter().GetResult();
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return responseBody;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }


        public static string HttpClientPost(string url, IList<KeyValuePair<string, string>> parameters, int httpClientTimeout = 10)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(httpClientTimeout);

                var content = new FormUrlEncodedContent(parameters);
                var response = httpClient.PostAsync(url, content).GetAwaiter().GetResult();
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return responseBody;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 发起POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="contentType">application/xml、application/json、application/text、application/x-www-form-urlencoded</param>
        /// <param name="headers">填充消息头</param>        
        /// <returns></returns>
        public static string HttpPostJson(string url, string postData = null, string contentType = "application/json", int timeOut = 30, Dictionary<string, string> headers = null)
        {
            postData = postData ?? "";
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, timeOut);
                if (headers != null)
                {
                    foreach (var header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                using (HttpContent httpContent = new StringContent(postData, Encoding.UTF8))
                {
                    if (contentType != null)
                        httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    HttpResponseMessage response = client.PostAsync(url, httpContent).GetAwaiter().GetResult();
                    return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
            }
        }

        /// <summary>
        /// 发起POST异步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="contentType">application/xml、application/json、application/text、application/x-www-form-urlencoded</param>
        /// <param name="headers">填充消息头</param>        
        /// <returns></returns>
        public static async Task<string> HttpPostJsonAsync(string url, string postData = null, string contentType = "application/json", int timeOut = 30, Dictionary<string, string> headers = null)
        {
            postData = postData ?? "";
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, timeOut);
                if (headers != null)
                {
                    foreach (var header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                using (HttpContent httpContent = new StringContent(postData, Encoding.UTF8))
                {
                    if (contentType != null)
                        httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    HttpResponseMessage response = await client.PostAsync(url, httpContent);
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
