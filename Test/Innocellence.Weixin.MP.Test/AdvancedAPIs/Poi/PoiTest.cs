using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.MP.AdvancedAPIs.Poi;
using Innocellence.Weixin.MP.CommonAPIs;
using Innocellence.Weixin.MP.Entities;
using Innocellence.Weixin.MP.Test.CommonAPIs;

namespace Innocellence.Weixin.MP.Test.AdvancedAPIs
{
    [TestClass]
    public class PoiTest : CommonApiTest
    {
        [TestMethod]
        public void UploadImageTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            string file = @"E:\1.jpg";

            var result = PoiApi.UploadImage(_appId, _appSecret, file);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void GetPoiListTest()
        {
            //var accessToken = AccessTokenContainer.GetAccessToken(_appId);

            var result = PoiApi.GetPoiList(_appId, _appSecret, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }
    }
}
