using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.MP.AdvancedAPIs.GroupMessage;
using Innocellence.Weixin.MP.AdvancedAPIs.Media;
using Innocellence.Weixin.MP.CommonAPIs;
using Innocellence.Weixin.MP.Test.CommonAPIs;

namespace Innocellence.Weixin.MP.Test.AdvancedAPIs
{
    //已测试通过
    [TestClass]
    public class MediaTest : CommonApiTest
    {
        private string mediaId = null;

        [TestMethod]
        public void UploadTemporaryMediaTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_appId,_appSecret);

            var type = UploadMediaFileType.image;
            var file = @"E:\1.jpg";
            var result = MediaApi.UploadTemporaryMedia(accessToken,_appSecret, type, file);

            Assert.AreEqual(type, result.type);
            Assert.IsNotNull(result.media_id);
            mediaId = result.media_id;
        }

        [TestMethod]
        public void UploadTemporaryNewsTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_appId, _appSecret);

            var file = @"E:\1.jpg";
            var mediaId = MediaApi.UploadTemporaryMedia(accessToken, _appSecret, UploadMediaFileType.thumb, file).thumb_media_id;

            Assert.IsNotNull(mediaId);

            var new1 = new NewsModel()
            {
                author = "test",
                content = "test",
                content_source_url = "http://qy.weiweihi.com/Content/Images/app/qyhelper.png",
                digest = "test",
                show_cover_pic = "1",
                thumb_media_id = mediaId,
                title = "test"
            };

            var new2 = new NewsModel()
            {
                author = "test",
                content = "test111",
                content_source_url = "http://qy.weiweihi.com/Content/Images/app/qyhelper.png",
                digest = "test",
                show_cover_pic = "1",
                thumb_media_id = mediaId,
                title = "test"
            };

            var result = MediaApi.UploadTemporaryNews(accessToken, _appSecret, 10000, new1, new2);

            Assert.IsNotNull(result.media_id);
        }

        [TestMethod]
        public void GetTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_appId, _appSecret);

            UploadTemporaryMediaTest();//上传

            using (MemoryStream ms = new MemoryStream())
            {
                MediaApi.Get(accessToken, mediaId, ms);
                Assert.IsTrue(ms.Length > 0);

                //保存到文件
                var fileName = string.Format(@"E:\testpic_{0}.jpg", DateTime.Now.Ticks);
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    ms.Position = 0;
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        fs.Write(buffer, 0, bytesRead);
                    }
                    fs.Flush();
                }

                Assert.IsTrue(File.Exists(fileName));
            }
        }


        private string UploadForeverMediaTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_appId, _appSecret);

            var file = @"..\..\AdvancedAPIs\Media\test.jpg";

            var result = MediaApi.UploadForeverMedia(accessToken, _appSecret, file);

            Assert.IsNotNull(result.media_id);
            mediaId = result.media_id;
            return mediaId;
        }

        [TestMethod]
        public void UploadForeverVideoTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_appId, _appSecret);

            var file = @"E:\Test.mp4";
            var result = MediaApi.UploadForeverVideo(accessToken, _appSecret, file, "测试", "测试");

            Assert.IsNotNull(result.media_id);
            mediaId = result.media_id;
        }

        //[TestMethod]
        private string UploadAndUpdateNewsTest(string accessToken)
        {
            var file = @"E:\1.jpg";
            var result = MediaApi.UploadForeverMedia(accessToken, _appSecret, file);

            Assert.IsNotNull(result.media_id);

            var new1 = new NewsModel()
                {
                    author = "test",
                    content = "test",
                    content_source_url = "http://qy.weiweihi.com/Content/Images/app/qyhelper.png",
                    digest = "test",
                    show_cover_pic = "1",
                    thumb_media_id = result.media_id,
                    title = "test"
                };

            var new2 = new NewsModel()
            {
                author = "test",
                content = "test111",
                content_source_url = "http://qy.weiweihi.com/Content/Images/app/qyhelper.png",
                digest = "test",
                show_cover_pic = "1",
                thumb_media_id = result.media_id,
                title = "test"
            };

            var result1 = MediaApi.UploadNews(accessToken, _appSecret, 10000, new1, new2);

            Assert.IsNotNull(result1.media_id);

            //var result2 = MediaApi.UpdateForeverNews(accessToken, result1.media_id, 0, 10000, new2);

            //Assert.AreEqual(result2.errcode, ReturnCode.请求成功);

            return result1.media_id;
        }

        [TestMethod]
        public void GetForeverMediaTest()
        {
            var mediaId = UploadForeverMediaTest();
            var accessToken = AccessTokenContainer.GetToken(_appId, _appSecret);
            using (MemoryStream stream = new MemoryStream())
            {
                MediaApi.GetForeverMedia(accessToken, mediaId, stream);
                Assert.IsTrue(stream.Length > 0);

                var fileName = @"..\..\AdvancedAPIs\Media\test.download." + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".jpg";
                using (var fs = new FileStream(fileName, FileMode.CreateNew))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fs);
                    fs.Flush();
                }
            }
        }

        //[TestMethod]
        private void GetForeverNewsTest(string accessToken, string mediaId)
        {
            var result = MediaApi.GetForeverNews(accessToken, _appSecret, mediaId);

            Assert.IsTrue(result.news_item.Count > 0);
            Assert.AreEqual(result.news_item[0].content, "test");
        }

        //[TestMethod]
        private void DeleteForeverMediaTest(string accessToken, string mediaId)
        {
            var result = MediaApi.DeleteForeverMedia(accessToken, _appSecret, mediaId);

            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void ForeverNewsTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_appId, _appSecret);

            string mediaId = UploadAndUpdateNewsTest(accessToken);
            GetForeverNewsTest(accessToken, mediaId);
            DeleteForeverMediaTest(accessToken, mediaId);
        }

        [TestMethod]
        public void GetMediaListTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_appId, _appSecret);

            var result = MediaApi.GetNewsMediaList(accessToken, _appSecret, 0, 5);

            Assert.IsNotNull(result.item_count);
        }

        [TestMethod]
        public void GetNewsMediaListTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_appId, _appSecret);

            var result = MediaApi.GetNewsMediaList(accessToken, _appSecret, 0, 3);

            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
            Assert.AreEqual(result.item_count, 3);
        }

        [TestMethod]
        public void GetOthersMediaListTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_appId, _appSecret);

            var result = MediaApi.GetOthersMediaList(accessToken, _appSecret, UploadMediaFileType.image, 0, 3);

            Assert.AreEqual(result.errcode, ReturnCode.请求成功);
            Assert.AreEqual(result.item_count, 3);
        }

        [TestMethod]
        public void AfterDeleteImgTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_appId, _appSecret);

            var file = @"..\..\AdvancedAPIs\Media\test.jpg";

            var result = MediaApi.UploadForeverMedia(accessToken, _appSecret, file);

            Assert.IsNotNull(result.media_id);

            CustomApi.SendImage(accessToken, _appSecret, "o3IHxjrPzMVZIJOgYMH1PyoTW_Tg", result.media_id);

            MediaApi.DeleteForeverMedia(accessToken, _appSecret, result.media_id);
        }

        [TestMethod]
        public void AfterDeleteNewsTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_appId, _appSecret);

            var file = @"E:\1.jpg";
            var result = MediaApi.UploadForeverMedia(accessToken, _appSecret, file);

            Assert.IsNotNull(result.media_id);

            var new1 = new NewsModel()
            {
                author = "test",
                content = "test",
                content_source_url = "http://qy.weiweihi.com/Content/Images/app/qyhelper.png",
                digest = "test",
                show_cover_pic = "1",
                thumb_media_id = result.media_id,
                title = "test"
            };

            var new2 = new NewsModel()
            {
                author = "test",
                content = "test111",
                content_source_url = "http://qy.weiweihi.com/Content/Images/app/qyhelper.png",
                digest = "test",
                show_cover_pic = "1",
                thumb_media_id = result.media_id,
                title = "test"
            };

            var result1 = MediaApi.UploadNews(accessToken, _appSecret, 10000, new1, new2);

            Assert.IsNotNull(result1.media_id);

            GroupMessageApi.SendGroupMessageByOpenId(accessToken, _appSecret, GroupMessageType.mpnews, result1.media_id, 10000, "o3IHxjrPzMVZIJOgYMH1PyoTW_Tg", "o3IHxjrPzMVZIJOgYMH1PyoTW_Tg");
            //var result2 = MediaApi.UpdateForeverNews(accessToken, result1.media_id, 0, 10000, new2);

            MediaApi.DeleteForeverMedia(accessToken, _appSecret, result1.media_id);
            //Assert.AreEqual(result2.errcode, ReturnCode.请求成功);
        }

        [TestMethod]
        public void AfterDeleteVideoTest()
        {
            var accessToken = AccessTokenContainer.GetToken(_appId, _appSecret);

            var file = @"E:\Test.mp4";
            var result = MediaApi.UploadForeverVideo(accessToken, _appSecret, file, "测试", "测试", 100000);

            Assert.IsNotNull(result.media_id);

            CustomApi.SendVideo(accessToken, _appSecret, "o3IHxjrPzMVZIJOgYMH1PyoTW_Tg", result.media_id, "测试", "测试");
            MediaApi.DeleteForeverMedia(accessToken, _appSecret, result.media_id);
        }
    }
}
