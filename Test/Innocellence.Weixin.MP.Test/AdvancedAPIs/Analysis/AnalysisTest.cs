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
    public class AnalysisTest : CommonApiTest
    {
        protected string beginData = "2015-3-9";
        protected string endData = "2015-3-9";

        [TestMethod]
        public void ArticleSummaryTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = AnalysisApi.GetArticleSummary(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void ArticleTotalTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetArticleTotal(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void UserReadTest()
        {
          //  var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetUserRead(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void UserReadHourTest()
        {
            //var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetUserReadHour(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void UserShareTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetUserShare(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void UserShareHourTest()
        {
            //var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetUserShareHour(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetInterfaceSummaryTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetInterfaceSummary(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetInterfaceSummaryHourTest()
        {
         //   var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetInterfaceSummaryHour(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetUpStreamMsgTest()
        {
          //  var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetUpStreamMsg(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetUpStreamMsgHourTest()
        {
          //  var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetUpStreamMsgHour(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetUpStreamMsgWeekTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetUpStreamMsgWeek(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetUpStreamMsgMonthTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetUpStreamMsgMonth(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetUpStreamMsgDistTest()
        {
          //  var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetUpStreamMsgDist(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetUpStreamMsgDistWeekTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetUpStreamMsgDistWeek(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetUpStreamMsgDistMonthTest()
        {
          //  var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetUpStreamMsgDistMonth(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetUserSummaryTest()
        {
          //  var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetUserSummary(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.list[0].ref_date != null);
        }

        [TestMethod]
        public void GetUserCumulateTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = AnalysisApi.GetUserCumulate(_appId, _appSecret, beginData, endData);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.list[0].ref_date != null);
        }
    }
}
