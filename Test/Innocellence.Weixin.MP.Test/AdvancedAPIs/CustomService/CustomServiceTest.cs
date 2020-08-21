using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.MP.AdvancedAPIs.CustomService;
using Innocellence.Weixin.MP.CommonAPIs;
using Innocellence.Weixin.MP.Helpers;
using Innocellence.Weixin.MP.Test.CommonAPIs;

namespace Innocellence.Weixin.MP.Test.AdvancedAPIs
{
    [TestClass]
    public class CustomServiceTest : CommonApiTest
    {
        protected string _custonPassWord = MD5UtilHelper.GetMD5("123123", null);

        [TestMethod]
        public void GetRecordTest()
        {
            var openId = "o3IHxjkke04__4n1kFeXpfMjjRBc";
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = CustomServiceApi.GetRecord(_appId, _appSecret, DateTime.Today, DateTime.Now, 10, 1);
            Assert.IsTrue(result.recordlist.Count > 0);
        }

        [TestMethod]
        public void GetCustomBasicInfoTest()
        {
          //  var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = CustomServiceApi.GetCustomBasicInfo(_appId, _appSecret);
            Assert.IsTrue(result.kf_list.Count > 0);
        }

        [TestMethod]
        public void GetCustomOnlineInfoTest()
        {
            //var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = CustomServiceApi.GetCustomOnlineInfo(_appId, _appSecret);
            Assert.IsTrue(result.kf_online_list.Count > 0);
        }

        [TestMethod]
        public void AddCustomTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = CustomServiceApi.AddCustom(_appId, _appSecret, "zcc@InnocellenceRobot", "zcc", _custonPassWord);
            Assert.AreEqual(result.errcode,ReturnCode.请求成功);
        }

        [TestMethod]
        public void UpdateCustomTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = CustomServiceApi.UpdateCustom(_appId, _appSecret, "zcc@InnocellenceRobot", "zcc", _custonPassWord);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void UploadCustomHeadimgCustom()
        {
            string file = "C:\\Users\\czhang\\Desktop\\logo.png";

           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = CustomServiceApi.UploadCustomHeadimg(_appId, _appSecret, "zcc@InnocellenceRobot", file);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void DeleteCustomTest()
        {
          //  var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = CustomServiceApi.DeleteCustom(_appId, _appSecret, "zcc@InnocellenceRobot");
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void CreateSessionTest()
        {
          //  var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = CustomServiceApi.CreateSession(_appId, _appSecret, _testOpenId, "zcc@InnocellenceRobot");
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void CloseSessionTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = CustomServiceApi.CloseSession(_appId, _appSecret, _testOpenId, "zcc@InnocellenceRobot");
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetSessionStateTest()
        {
          //  var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = CustomServiceApi.GetSessionState(_appId, _appSecret, _testOpenId);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetSessionListTest()
        {
          //  var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = CustomServiceApi.GetSessionList(_appId, _appSecret, _testOpenId);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetWaitCaseTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = CustomServiceApi.GetWaitCase(_appId, _appSecret);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }
    }
}
