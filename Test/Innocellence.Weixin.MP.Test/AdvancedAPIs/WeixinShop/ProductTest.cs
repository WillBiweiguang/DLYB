using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.MP.AdvancedAPIs.MerChant;
using Innocellence.Weixin.MP.CommonAPIs;
using Innocellence.Weixin.MP.Entities;
using Innocellence.Weixin.MP.Test.CommonAPIs;
using Innocellence.Weixin.MP.TenPayLib;

namespace Innocellence.Weixin.MP.Test.AdvancedAPIs
{
    //需要通过微信支付审核的账户才能成功
    [TestClass]
    public class ProductTest : CommonApiTest
    {
        [TestMethod]
        public void AddProdectTest()
        {
            var addProductData = new AddProductData();
            var result = ProductApi.AddProduct("[appid]", addProductData);
            Console.Write(result);
            Assert.IsNotNull(result);
        }
    }
}
