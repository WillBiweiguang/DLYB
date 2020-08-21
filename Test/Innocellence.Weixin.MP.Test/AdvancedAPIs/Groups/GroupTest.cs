using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.MP.AdvancedAPIs.Groups;
using Innocellence.Weixin.MP.CommonAPIs;
using Innocellence.Weixin.MP.Entities;
using Innocellence.Weixin.MP.Test.CommonAPIs;

namespace Innocellence.Weixin.MP.Test.AdvancedAPIs
{
    //已经通过测试
    [TestClass]
    public class GroupTest : CommonApiTest
    {
        [TestMethod]
        public void CreateTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = GroupsApi.Create(_appId, _appSecret, "测试组");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.group.id >= 100);
        }

        [TestMethod]
        public void GetTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = GroupsApi.Get(_appId, _appSecret);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.groups.Count >= 3);
        }

        [TestMethod]
        public void GetIdTest()
        {
          //  var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = GroupsApi.GetId(_appId, _appSecret, _testOpenId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.groupid >= 0);
        }

        [TestMethod]
        public void UpdateTest()
        {
         //   var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = GroupsApi.Update(_appId, _appSecret, 100, "测试组更新");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.errcode == ReturnCode.请求成功);
        }

        [TestMethod]
        public void MemberUpdateTest()
        {
            //var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var idArr = new[] { 0, 1, 2, 100, 0 };
            foreach (var id in idArr)
            {
                var result = GroupsApi.MemberUpdate(_appId, _appSecret, _testOpenId, id);
                Assert.IsNotNull(result);
                Assert.IsTrue(result.errcode == ReturnCode.请求成功);
                var newGroupIdResult = GroupsApi.GetId(_appId, _appSecret, _testOpenId);
                Assert.AreEqual(id, newGroupIdResult.groupid);
            }
        }
    }
}
