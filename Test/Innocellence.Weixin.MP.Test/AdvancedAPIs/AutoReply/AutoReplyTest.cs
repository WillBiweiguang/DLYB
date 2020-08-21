using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.MP.AdvancedAPIs.Analysis;
using Innocellence.Weixin.MP.CommonAPIs;
using Innocellence.Weixin.MP.Entities;
using Innocellence.Weixin.MP.Test.CommonAPIs;

namespace Innocellence.Weixin.MP.Test.AdvancedAPIs
{
    //已经通过测试
    [TestClass]
    public class AutoReplyTest : CommonApiTest
    {
        [TestMethod]
        public void ArticleSummaryTest()
        {
            //var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = AutoReplyApi.GetCurrentAutoreplyInfo(_appId, _appSecret);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }
    }
}
