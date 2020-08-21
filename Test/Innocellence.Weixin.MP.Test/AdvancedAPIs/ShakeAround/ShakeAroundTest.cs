using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.MP.AdvancedAPIs.ShakeAround;
using Innocellence.Weixin.MP.CommonAPIs;
using Innocellence.Weixin.MP.Entities;
using Innocellence.Weixin.MP.Test.CommonAPIs;

namespace Innocellence.Weixin.MP.Test.AdvancedAPIs
{
    [TestClass]
    public class ShakeAroundTest : CommonApiTest
    {
        [TestMethod]
        public void DeviceApplyTest()
        {
            //var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = ShakeAroundApi.DeviceApply(_appId, _appSecret, 1, "测试", "测试");
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void UploadImageTest()
        {
            //var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            string file = @"E:\测试.jpg";

            var result = ShakeAroundApi.UploadImage(_appId, _appSecret, file);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }
    }
}
