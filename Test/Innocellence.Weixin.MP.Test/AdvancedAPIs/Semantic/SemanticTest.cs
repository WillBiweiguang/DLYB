﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.MP.AdvancedAPIs.Semantic;
using Innocellence.Weixin.MP.CommonAPIs;
using Innocellence.Weixin.MP.Test.CommonAPIs;

namespace Innocellence.Weixin.MP.Test.AdvancedAPIs
{
    [TestClass]
    public class SemanticTest : CommonApiTest
    {
        protected SemanticPostData SemanticPostData = new SemanticPostData()
            {
                query = "查一下明天从北京到上海的南航机票",
                category = "flight",
                city = "北京",
                appid = "wxbe855a981c34aa3f",
                uid = "123456"
            };

        [TestMethod]
        public void SemanticUnderStandTest()
        {
           // var accessToken = AccessTokenContainer.GetAccessToken(_appId);
            var result = SemanticApi.SemanticSend<Semantic_RestaurantResult>(_appId, _appSecret, SemanticPostData);
            
            Assert.IsNotNull(result.query);
            Assert.AreEqual("附近有什么川菜馆", result.query);
        }

        [TestMethod]
        public void RestaurantResultTest()
        {
            string returnText = "{\"res\":0,\"query\":\" 附近有什么川菜馆\",\"type\":\"restaurant\",\"semantic\":{\"details\":{\"category\":\"川菜\"},\"intent\":\"SEARCH\"}}";
            var result = Innocellence.Weixin.HttpUtility.Post.GetResult<Semantic_RestaurantResult>(returnText);
            Assert.IsNotNull(result.semantic);
            Assert.AreEqual(" 附近有什么川菜馆", result.query);
            Assert.AreEqual("SEARCH", result.semantic.intent);
            Assert.AreEqual("川菜", result.semantic.details.category);
        }

    }
}
