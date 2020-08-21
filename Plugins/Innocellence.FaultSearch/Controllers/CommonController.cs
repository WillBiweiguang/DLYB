using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.ImageTools;
using DLYB.CA.Entity;
using NetUtilityLib;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;


namespace DLYB.CA.Controllers
{
    public class CommonController : Controller //WeChatBaseController<ArticleInfo, ArticleInfoView>
    {
        private static string baseUrl = WebConfigurationManager.AppSettings["WebUrl"];
        private static string cmsHost = CommonService.GetSysConfig("Content Server", "");

        protected BaseService<ArticleImages> _articelImageService = new BaseService<ArticleImages>("CAAdmin");
        protected BaseService<QuestionImages> _questionImageService = new BaseService<QuestionImages>("CAAdmin");

        private static object lockerObj = new object();

        public ActionResult Error()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SetCurApp(string CurAppID)
        {
            Session["AppID"] = CurAppID;
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ThumbFile(int? Id, string FileName)
        {
            return CommonThumbFile<ArticleImages>(Id, FileName);
        }

        public ActionResult ThumbQuestionFile(int? Id, string FileName)
        {
            return CommonThumbFile<QuestionImages>(Id, FileName);
        }

        private ActionResult CommonThumbFile<T>(int? Id, string FileName) where T : EntityBase<int>
        {
            if (!Id.HasValue && string.IsNullOrEmpty(FileName))
            {
                LogManager.GetLogger(GetType())
                    .Error("Id and Filename are all empty! now returning default LillyLogoRed.png image!");
                return File(cmsHost + "Content/img/LillyLogoRed.png", "image/jpeg");
            }
            // 健壮性改造
            try
            {

                string strFullPath = Server.MapPath("/" + FileName);
                string thumbFullPath = string.Empty;
                string thumbFilename = Path.GetFileNameWithoutExtension(strFullPath) + "_T";
                string fileExt = Path.GetExtension(strFullPath);
                string dir = Path.GetDirectoryName(strFullPath);

                thumbFullPath = Path.Combine(new string[] { dir, thumbFilename + fileExt });

                LogManager.GetLogger(GetType()).Debug("Common.File, strPath=" + strFullPath);
                if (!System.IO.File.Exists(thumbFullPath))
                {
                    //strPath = strPath.Replace('/', '\\').Replace(@"\\", @"\");
                    LogManager.GetLogger(GetType()).Debug("Thumbnail Image File not exists. Now Saving...");
                    BaseService<T> ser = new BaseService<T>("CAAdmin");
                    dynamic obj = ser.Repository.GetByKey(Id.Value);
                    if (obj != null)
                    {
                        // 修正从DB取得的图片与URL图片不符问题
                        // DB如果没有图片信息，使用默认图片
                        if (string.IsNullOrEmpty(obj.ImageName))
                        {
                            LogManager.GetLogger(GetType())
                                .Error("The image path in Database was empty!");
                            return File(cmsHost + "Content/img/LillyLogoRed.png", "image/jpeg");
                        }

                        strFullPath = Server.MapPath("/" + obj.ImageName);
                        thumbFilename = Path.GetFileNameWithoutExtension(strFullPath) + "_T";
                        fileExt = Path.GetExtension(strFullPath);
                        dir = Path.GetDirectoryName(strFullPath);
                        thumbFullPath = Path.Combine(new string[] { dir, thumbFilename + fileExt });

                        using (MemoryStream ms = new MemoryStream(obj.ImageContent))
                        {
                            if (!System.IO.File.Exists(strFullPath))
                            {
                                // 顺便保存一下原始文件
                                LogManager.GetLogger(GetType()).Debug("Original Image File not exists. Now Saving...");
                                using (FileStream fs = new FileStream(strFullPath, FileMode.Create))
                                {
                                    fs.Write(obj.ImageContent, 0, obj.ImageContent.Length);
                                    fs.Close();
                                }
                            }

                            if (!System.IO.File.Exists(thumbFullPath))
                            {
                                var thumbnailImage = ImageUtility.MakeThumbnail(null, ms, thumbFullPath, 200, 0, "W", 0 ,true);//update by anrew. 6/12
                            }

                        }
                    }
                }

                return File(thumbFullPath, "image/jpeg");
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(GetType())
                    .Debug("Exception in getting picture, returning :" + cmsHost + "Content/img/LillyLogoRed.png" +
                           "\t\r\nException: " + ex.Message + "\t\r\n" + ex.StackTrace);

                return File(cmsHost + "Content/img/LillyLogoRed.png", "image/jpeg");
            }
        }

        /// <summary>
        /// http://ddddd.com/Common/PushFile?id=1&FileName=./image/dddd&ImgType=1
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FileName"></param>
        /// <param name="ImgType"></param>
        /// <returns></returns>
        public ActionResult PushFile(int? Id, string FileName, string ImgType)
        {
            LogManager.GetLogger(GetType()).Debug("Enter PushFile!");

            // 修正从DB取得的图片与URL图片不符问题
            if (!Id.HasValue && string.IsNullOrEmpty(FileName))
            {
                LogManager.GetLogger(GetType())
                    .Error("Id and Filename are all empty! now returning default LillyLogoRed.png image!");
                return File(cmsHost + "Content/img/LillyLogoRed.png", "image/jpeg");
            }

            try
            {
                                
                //if (Id.HasValue)
                //{
                string strPath = Server.MapPath("/" + FileName);

                LogManager.GetLogger(GetType()).Debug("Common.File, strPath=" + strPath);

                if (!System.IO.File.Exists(strPath))
                {
                    LogManager.GetLogger(GetType()).Debug("File not exists.");
                    BaseService<ArticleImages> ser = new BaseService<ArticleImages>("CAAdmin");
                    var obj = ser.Repository.GetByKey(Id.Value);
                    if (obj != null)
                    {
                        if (string.IsNullOrEmpty(obj.ImageName))
                        {
                            LogManager.GetLogger(GetType())
                                .Error("The image path in Database was empty!");
                            return File(cmsHost + "Content/img/LillyLogoRed.png", "image/jpeg");
                        }

                        // 正确获取数据库上图片信息
                        strPath = Server.MapPath("/" + obj.ImageName);

                        if (!System.IO.File.Exists(strPath))
                        {
                            CheckAndCreateDirectory(strPath);

                            lock (lockerObj)
                            {
                                using (FileStream fs = new FileStream(strPath, FileMode.Create))
                                {
                                    fs.Write(obj.ImageContent, 0, obj.ImageContent.Length);
                                    fs.Close();
                                    Thread.Sleep(100);
                                }
                            }
                        }
                    }
                }

                return File(strPath, "image/jpeg");//not sure
                //}
                //else
                //{
                //    return File(cmsHost + "Content/img/LillyLogoRed.png", "image/jpeg");
                //}
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(GetType())
                    .Debug("Exception in getting picture, returning :" + cmsHost + "Content/img/LillyLogoRed.png" +
                           "\t\r\nException: " + ex.Message + "\t\r\n" + ex.StackTrace);

                return File(cmsHost + "Content/img/LillyLogoRed.png", "image/jpeg");
            }
        }

        /// <summary>
        /// http://ddddd.com/Common/File?id=1&FileName=./image/dddd&ImgType=1
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FileName"></param>
        /// <param name="ImgType"></param>
        /// <returns></returns>
        public ActionResult File(int? Id, string FileName, string ImgType)
        {

           return CommonFile<ArticleImages>(Id, FileName, ImgType);

        }

        /// <summary>
        /// http://ddddd.com/Common/File?id=1&FileName=./image/dddd&ImgType=1
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FileName"></param>
        /// <param name="ImgType"></param>
        /// <returns></returns>
        public ActionResult QuestionFile(int? Id, string FileName, string ImgType)
        {
            return CommonFile<QuestionImages>(Id, FileName, ImgType, false);
        }

        /// <summary>
        /// http://ddddd.com/Common/File?id=1&FileName=./image/dddd&ImgType=1
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FileName"></param>
        /// <param name="ImgType"></param>
        /// <returns></returns>
        public ActionResult CommonFile<T1>(int? Id, string FileName, string ImgType, bool watermark = true) where T1:EntityBase<int>
        {

            // 修正从DB取得的图片与URL图片不符问题
            if (!Id.HasValue && string.IsNullOrEmpty(FileName))
            {
                LogManager.GetLogger(GetType())
                    .Error("Id and Filename are all empty! now returning default LillyLogoRed.png image!");
                return File(cmsHost + "Content/img/LillyLogoRed.png", "image/jpeg");
            }
            //if (Id.HasValue)
            //{
            string strPath = Server.MapPath("/" + FileName);

            LogManager.GetLogger(GetType()).Debug("Common.File, strPath=" + strPath);


            if (!System.IO.File.Exists(strPath))
            {
                LogManager.GetLogger(GetType()).Debug("File not exists.");
                BaseService<T1> service = new BaseService<T1>("CAAdmin");

                dynamic obj = service.Repository.GetByKey(Id.Value);
                if (obj != null)
                {
                    if (string.IsNullOrEmpty(obj.ImageName))
                    {
                        LogManager.GetLogger(GetType())
                            .Error("The image path in Database was empty!");
                        return File(cmsHost + "Content/img/LillyLogoRed.png", "image/jpeg");
                    }

                    // 正确获取数据库上图片信息
                    strPath = Server.MapPath("/" + obj.ImageName);

                    if (!System.IO.File.Exists(strPath))
                    {
                        CheckAndCreateDirectory(strPath);

                        using (FileStream fs = new FileStream(strPath, FileMode.Create))
                        {
                            fs.Write(obj.ImageContent, 0, obj.ImageContent.Length);
                            fs.Close();
                        }
                    }
                }
                else
                {
                    return File(cmsHost + "Content/img/LillyLogoRed.png", "image/jpeg");
                }
            }

            if (!watermark)
            {
                return File(strPath, "image/jpeg");
            }

            Image wmImage;
            Image img = new Bitmap(strPath);
            try
            {
                // TODO: 先尝试实时添加水印的效果
                if (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
                {

                    LogManager.GetLogger(GetType()).Debug("User.Identity found: " + User.Identity.Name);
                    wmImage = ImageUtility.AddWatermarkText(img, User.Identity.Name, ImageAlign.RightBottom);

                    return File(ImageHelper.ImageToBytes(wmImage), "image/jpeg");
                }
                else
                {
                    // 否则暂时加个固定水印吧
                    wmImage = ImageUtility.AddWatermarkText(img, "仅供内部参考", ImageAlign.RightBottom);
                    LogManager.GetLogger(GetType())
                        .Debug("User Identity not found, returning strPath + Internal Only:" + strPath);
                    return File(ImageHelper.ImageToBytes(wmImage), "image/jpeg");
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(GetType())
                    .Debug("Exception in adding water mark, returning :" + cmsHost + "Content/img/LillyLogoRed.png" +
                           "\t\r\nException: " + ex.Message + "\t\r\n" + ex.StackTrace);
                return File(cmsHost + "Content/img/LillyLogoRed.png", "image/jpeg");
            }

        }

        /// <summary>
        /// 判定目标路径是否存在，不存在则创建对应文件夹
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        private void CheckAndCreateDirectory(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string lineType = filePath.Contains(@"/") ? @"/" : @"\";
                string directoryPath = filePath.Substring(0, filePath.LastIndexOf(lineType) + 1);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
        }

        /// <summary>
        /// 上传图片处理
        /// </summary>
        /// <returns></returns>
        public ActionResult PostImage()
        {
            string strPath = "";
            string isVideo = Request["isVideo"];//For video poster.
            byte[] uploadFileBytes = null;
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase objFile = Request.Files[0];

                var tmpPath = Server.MapPath("~/temp");

                var strExt = System.IO.Path.GetExtension(objFile.FileName);

                strPath = string.Format("{0}\\{1}**{2}", tmpPath, DateTime.Now.ToString("yyyyMMddHHmmss"), strExt);
                
                Image imageThumb;
                 
                if (!string.IsNullOrEmpty(strExt) && ".jpg,.png,.bmp,.jpeg,.gif".IndexOf(strExt.ToLower()) >= 0)
                {
                    //生成缩略图

                    imageThumb = ImageUtility.MakeThumbnail(null, objFile.InputStream, strPath.Replace("**", ""), !string.IsNullOrEmpty(isVideo) ? 900 : 400, 225, "W", 1);

                    if (imageThumb != null)
                    {
                        uploadFileBytes = ImageHelper.ImageToBytes(Image.FromFile(strPath.Replace("**", "")));
                    }
                }
            }

            string strName = System.IO.Path.GetFileName(strPath.Replace("**", ""));

            ArticleImages image = new ArticleImages()
            {
                ImageContent = uploadFileBytes,
                ImageName = "/temp/" + strName
            };

            _articelImageService.Repository.Insert(image);

            string imgUrl = string.Format(image.ImageName);
            return Json(new UploadMessageSuccess(new UploadMessageSuccessMsg(strName, imgUrl), image.Id.ToString(CultureInfo.InvariantCulture)),
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 问题上传图片处理
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> PostQuestionImage()
        {

            using (MemoryStream ms = new MemoryStream())
            {

                int read = 0;
                byte[] buff = new byte[512];
                while ((read = Request.InputStream.Read(buff, 0, buff.Length)) > 0)
                {
                    ms.Write(buff, 0, read);
                }

                var imageMemory = Image.FromStream(ms);
                var imagetype = "";
                if (ImageFormat.Jpeg.Equals(imageMemory.RawFormat))
                {
                    imagetype = ".JPEG";
                }
                else if (ImageFormat.Png.Equals(imageMemory.RawFormat))
                {
                    imagetype = ".PNG";
                }
                else if (ImageFormat.Gif.Equals(imageMemory.RawFormat))
                {
                    imagetype = ".GIF";
                }
                else if (ImageFormat.Bmp.Equals(imageMemory.RawFormat))
                {
                    imagetype = ".BMP";
                }
                string strPath = "";
                var tmpPath = Server.MapPath("~/temp");
                strPath = string.Format("{0}\\{1}**{2}", tmpPath, DateTime.Now.ToString("yyyyMMddHHmmss"), imagetype);

                if (ms.Length > 0)
                {

                    FileStream fileStream = null;
                    try
                    {
                        fileStream =
                            System.IO.File.Create(strPath.Replace("**", ""));
                        await fileStream.WriteAsync(ms.ToArray(), 0, (int)ms.Length);

                        if (".jpg,.png,.bmp,.jpeg,.gif".IndexOf(imagetype.ToLower()) >= 0)
                        {
                            //生成缩略图
                            Request.InputStream.Position = 0;
                            ImageUtility.MakeThumbnail(null, Request.InputStream, strPath.Replace("**", "_T"), 160, 120, "W");
                        }

                    }
                    catch (Exception ex)
                    {

                        return Json(new UploadMessageError("文件流为空或者生成缩略图失败", "", ""),
                        JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        if (fileStream != null)
                        {
                            fileStream.Close();
                        }
                    }

                }
                string strName = System.IO.Path.GetFileName(strPath.Replace("**", ""));

                QuestionImages image = new QuestionImages()
                {
                    ImageContent = ms.ToArray(),
                    ImageName = "/temp/" + strName
                };
                _questionImageService.Repository.Insert(image);

                string imgUrl = string.Format(image.ImageName);
                return Json(
                    new UploadMessageSuccess(new UploadMessageSuccessMsg(strName, imgUrl), image.Id.ToString()),
                    JsonRequestBehavior.AllowGet);
            }
        }

    }
}