// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 17:21</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Core;
using Infrastructure.Core.Data;
using DLYB.Web.Entity;
using DLYB.Web.Service;
using System.Linq.Expressions;
//using DLYB.Web.ModelsView;
using DLYB.Web.Models.Plugins;
using Infrastructure.Web.Helpers;
using Newtonsoft.Json;
using Innocellence.Web.Domain.Models;
using System.Net.Http;

namespace DLYB.CA.Services
{
    public partial class LoginService : ILoginService
    {
        private const string LoginApiUrl = "login";
        private const string CaptchApiUrl = "captcha";
        public bool Login(string userName, string password, string uuid = "", string captcha = "123")
        {
            string baseApiUrl = "http://quota.xpro.work/";
            string url = baseApiUrl + LoginApiUrl;
            var parameters = new
            {
                captcha = captcha,
                password = password,
                username = userName,
                uuid = string.IsNullOrEmpty(uuid) ? Guid.NewGuid().ToString() : uuid
            };
            var result = HttpClientHelper.HttpPostJson(url, JsonConvert.SerializeObject(parameters));
            var resultModel = JsonConvert.DeserializeObject<ApiResultModel>(result);
            return resultModel.code == Infrastructure.Web.Domain.Common.ApiReturnCode.Success;
        }

        public string GetCaptcha(string uuid)
        {
            string baseApiUrl = "http://quota.xpro.work/";
            string url = baseApiUrl + CaptchApiUrl + "&uuid=" + uuid;
            var httpClient = new HttpClient();
            var result = httpClient.GetAsync(url).GetAwaiter().GetResult();
            var bytes = result.Content.ReadAsByteArrayAsync().Result;
            return Convert.ToBase64String(bytes);
        }
    }
}