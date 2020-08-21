using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innocellence.Weixin.Exceptions;
using Innocellence.Weixin.QY.AdvancedAPIs;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;
using Innocellence.Weixin.QY.CommonAPIs;
using Innocellence.Weixin.QY.Test.CommonApis;

namespace Innocellence.Weixin.QY.Test.AdvancedAPIs
{
    /// <summary>
    /// CommonApiTest 的摘要说明
    /// </summary>
    [TestClass]
    public partial class DepartmentTest : CommonApiTest
    {
        //[TestMethod]
        public int CreateDepartmentTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_corpId,_corpSecret);
            var result = MailListApi.CreateDepartment(accessToken, "test", 1, id: 3);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.id > 0);
            return result.id;
        }

        //[TestMethod]
        public void UpdateDepartmentTest(string id)
        {
            var accessToken = AccessTokenContainer.GetToken(_corpId,_corpSecret);
            var result = MailListApi.UpdateDepartment(accessToken, id, "更新test", 1);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.errcode ==ReturnCode_QY.请求成功);
        }

        //[TestMethod]
        public void DeleteDepartmentTest(string id)
        {
            var accessToken = AccessTokenContainer.GetToken(_corpId, _corpSecret);
            var result = MailListApi.DeleteDepartment(accessToken, id);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.errcode == ReturnCode_QY.请求成功);
        }

        //[TestMethod]
        public void GetDepartmentListTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_corpId, _corpSecret);
            var result = MailListApi.GetDepartmentList(accessToken, 4);
            var result1 = MailListApi.GetDepartmentList(accessToken);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result1);
            Assert.IsTrue(result.errcode == ReturnCode_QY.请求成功);
        }

        [TestMethod]
        public void TestAllSet()
        {
            string id = CreateDepartmentTest().ToString();
            UpdateDepartmentTest(id);
            DeleteDepartmentTest(id);
            GetDepartmentListTest();
        }
    }
}
