using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.MP.CommonAPIs;
using Innocellence.Weixin.MP.Entities;
using Innocellence.Weixin.MP.Test.CommonAPIs;
using Innocellence.Weixin.MP.TenPayLib;

namespace Innocellence.Weixin.MP.Test.AdvancedAPIs
{
    //需要通过微信支付审核的账户才能成功
    [TestClass]
    public class TenPayRightsTest : CommonApiTest
    {
        [TestMethod]
        public void UpDateFeedBackTest()
        {
            var result = TenPayRights.UpDateFeedBack("[accessToken]", "[openId]", "[feedBackId]");
            Console.Write(result);
            Assert.IsNotNull(result);
        }
    }
}
