using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using Infrastructure.Core.Logging;


namespace Infrastructure.Web.ImageTools
{
    public class ImageUtility
    {
        string strInceptFile, strFilePath, strFileTurePath, strFilePathPv;
        int iFileMaxSize, iFileNameFlag;
        HttpFileCollection lstUploadFileObj;
        string[] arrUploadFileNames;
        int iErrCodes;
        int iContentLength;

        public List<FileInfoStruc> lstUploadFiles;


        public ImageUtility()
        {

            IsDeleteSImg = true;
            MaxType = 1;
            InceptMaxFile = 1;
            MaxSize = 4000;
            PreviewImageWidth = 200;
            PreviewImageHeight = 200;
            iErrCodes = 0;
            IsPreview = 1;
            PreviewType = PreviewTypes.Cut;
            RNamePv = "m_";
            InceptFileType = "gif, jpg, jpeg, png, bmp";
            lstUploadFiles = new List<FileInfoStruc>();

        }

        /// <summary> 
        /// 设置压缩类型 (1 压缩 2 智能压缩 3 不压缩) 
        /// </summary> 
        /// <value></value> 
        /// <remarks></remarks> 
        public int MaxType
        {
            set;
            get;
        }

        /// <summary> 
        /// 设置上传类型属性 (以逗号分隔多个文件类型) 
        /// </summary> 
        /// <value></value> 
        /// <remarks></remarks> 
        public string InceptFileType
        {
            set { strInceptFile = value.ToLower(); }
            get { return strInceptFile; }
        }

        /// <summary> 
        /// 设置上传文件大小上限 (单位：kb) 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public int MaxSize
        {
            get { return iFileMaxSize; }
            set { iFileMaxSize = value * 1024; }
        }

        /// <summary> 
        /// 设置每次上传文件上限 
        /// </summary> 
        /// <value></value> 
        /// <remarks></remarks> 
        public int InceptMaxFile
        {
            set;
            get;
        }

        /// <summary> 
        /// 设置上传目录路径(相对路径) 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public string UploadPath
        {
            get { return strFilePath; }
            set
            {
                // FilePath = value.Replace( Strings.Chr(0), "");
                // if (FilePath.(FilePath, 1) != "/") FilePath = FilePath + "/";
                strFilePath = value;
                if (!strFilePath.EndsWith("/")) { strFilePath = strFilePath + "/"; }
            }
        }

        /// <summary> 
        /// 设置上传目录路径(绝对路径) 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public string UploadTruePath
        {
            get { return strFileTurePath; }
            set
            {
                strFileTurePath = value;
                //FileTurePath = Strings.Replace(value, Strings.Chr(0), "");
                if (!strFileTurePath.EndsWith("/")) { strFileTurePath = strFileTurePath + "/"; }
            }
        }

        /// <summary> 
        /// 设置上传文件文件名方式(0 自动 1 原文件名) 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public int FileNameFlag
        {
            get { return iFileNameFlag; }
            set { iFileNameFlag = (value > 1 | value < 0 ? 0 : value); }
        }

        /// <summary> 
        /// 设置上传文件文件名方式(0 自动 1 原文件名) 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public string FileName
        {
            get;
            set;
        }

        /// <summary> 
        /// 设置上传目录路径(相对路径) 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public string UploadPvPath
        {
            get { return (string.IsNullOrEmpty(strFilePathPv) ? strFilePath : strFilePathPv); }
            set
            {
                strFilePathPv = value;
                // FilePathPv = Strings.Replace(value, Strings.Chr(0), "");
                if (!strFilePathPv.EndsWith("/")) { strFilePathPv = strFilePathPv + "/"; }
            }
        }

        /// <summary> 
        /// 设置上传文件 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public HttpFileCollection FileObjs
        {
            get { return lstUploadFileObj; }
            set { lstUploadFileObj = value; }
        }

        /// <summary> 
        /// 设置文件 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public string[] FileArr
        {
            get { return arrUploadFileNames; }
            set { arrUploadFileNames = value; }
        }

        /// <summary> 
        /// 设置文件名前缀 
        /// </summary> 
        /// <value></value> 
        /// <remarks></remarks> 
        public string RName
        {
            set;
            get;
        }

        /// <summary> 
        /// 设置文件名前缀(缩略图) 
        /// </summary> 
        /// <value></value> 
        /// <remarks></remarks> 
        public string RNamePv
        {
            set;
            get;
        }


        /// <summary> 
        /// 设置是否创建缩略图片(0=不创建，1=创建 默认创建) 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public int IsPreview
        {
            get;
            set;
        }

        /// <summary> 
        /// 设置是否创建缩略图片(0=不创建，1=创建 默认创建) 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public PreviewTypes PreviewType
        {
            get;
            set;
        }


        /// <summary>
        /// 0 Auto 1Width 2Height
        /// </summary>
        public int ScaleType
        {
            get;
            set;
        }

        /// <summary> 
        /// 设置缩略图片宽度属性 
        /// </summary> 
        /// <value></value> 
        /// <remarks></remarks> 
        public int PreviewImageWidth
        {
            get;
            set;
        }


        /// <summary> 
        /// 设置缩略图片高度属性 
        /// </summary> 
        /// <value></value> 
        /// <remarks></remarks> 
        public int PreviewImageHeight
        {
            get;
            set;
        }

        /// <summary> 
        /// 获取错误信息 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public string Description
        {
            get
            {
                switch (iErrCodes)
                {
                    case 1:
                        return "不支持 上传，服务器可能未安装该组件。";
                    //break;
                    case 2:
                        return "暂未选择上传组件！";
                    //break;
                    case 3:
                        return "请先选择你要上传的文件!";
                    //break;
                    case 4:
                        return "文件大小超过了限制 " + (iFileMaxSize / 1024) + "KB!";
                    //break;
                    case 5:
                        return "文件类型不正确!";
                    //break;
                    case 6:
                        return "已达到上传数的上限！";
                    //break;
                    case 7:
                        return "请不要重复提交！";
                    // break;
                    case 8:
                        return "文件不存在或者文件大小为0！";
                    //break;
                    default:
                        return null;
                    //break;
                }
            }
        }

        /// <summary>
        /// 水印的种类 0:无水印 1：图片水印 2：文字水印 3：混合水印
        /// </summary>
        public int WaterMarkType { get; set; }

        /// <summary>
        /// 是否删除原图
        /// </summary>
        public bool IsDeleteSImg { get; set; }

        /// <summary>
        /// 水印的相对位置
        /// </summary>
        public ImageAlign WaterMarkImgAlign { get; set; }

        /// <summary>
        /// 水印的绝对位置X坐标
        /// </summary>
        public int WaterMarkImgPlaceX { get; set; }

        /// <summary>
        /// 水印的绝对位置Y坐标
        /// </summary>
        public int WaterMarkImgPlaceY { get; set; }

        /// <summary>
        /// 水印旋转的角度
        /// </summary>
        public int WaterMarkImgAngle { get; set; }

        /// <summary>
        /// 水印的相对位置
        /// </summary>
        public ImageAlign WaterMarkTextAlign { get; set; }

        /// <summary>
        /// 水印的绝对位置X坐标
        /// </summary>
        public int WaterMarkTextPlaceX { get; set; }

        /// <summary>
        /// 水印的绝对位置Y坐标
        /// </summary>
        public int WaterMarkTextPlaceY { get; set; }

        /// <summary>
        /// 水印旋转的角度
        /// </summary>
        public int WaterMarkTextAngle { get; set; }


        /// <summary>
        /// 水印图片的背景颜色
        /// </summary>
        public string WaterMarkImageGroundColor { get; set; }

        /// <summary>
        /// 图片水印的水印图片路径
        /// </summary>
        public string WaterMarkImagePath { get; set; }

        /// <summary>
        /// 文字水印内容
        /// </summary>
        public string WaterMarkText { get; set; }

        /// <summary>
        /// 文字水印的文字颜色
        /// </summary>
        public string WaterMarkTextColor { get; set; }

        /// <summary>
        /// 文字水印的水印文字样式
        /// </summary>
        public TextCSS WaterMarkTextCSS { get; set; }

        /// <summary>
        /// 文字水印的水印文字字体
        /// </summary>
        public string WaterMarkTextFont { get; set; }

        /// <summary>
        /// 文字水印的水印文字字号
        /// </summary>
        public int WaterMarkTextSize { get; set; }

        /// <summary>
        /// 图片水印的透明度
        /// </summary>
        public int WaterMarkImgTransparence { get; set; }

        /// <summary>
        /// 文字水印的透明度
        /// </summary>
        public int WaterMarkTextTransparence { get; set; }


        /// <summary> 
        /// 判断文件类型:0=其它,1=图片,2=FLASH,3=音乐,4=电影 
        /// </summary> 
        /// <param name="FileExt"></param> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        private int CheckFiletype(string FileExt)
        {
            int functionReturnValue = 0;
            FileExt = FileExt.Replace(".", "");
            //FileExt = Strings.LCase(Strings.Replace(FileExt, ".", ""));
            switch (FileExt)
            {
                case "gif":
                case "jpg":
                case "jpeg":
                case "png":
                case "bmp":
                case "tif":
                case "iff":
                    functionReturnValue = 1;
                    break;
                case "swf":
                case "swi":
                    functionReturnValue = 2;
                    break;
                case "mid":
                case "wav":
                case "mp3":
                case "rmi":
                case "cda":
                    functionReturnValue = 3;
                    break;
                case "avi":
                case "mpg":
                case "mpeg":
                case "ra":
                case "ram":
                case "wov":
                case "asf":
                    functionReturnValue = 4;
                    break;
                default:
                    functionReturnValue = 0;
                    break;
            }
            return functionReturnValue;
        }

        /// <summary> 
        /// 日期时间定义文件名 
        /// </summary> 
        /// <param name="FileExt"></param> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public static string FormatName(string FileExt)
        {
            int RanNum = 0;
            string TempStr = "";
            Random Rnd = new Random();
            RanNum = Rnd.Next(100000, 900000);
            //TempStr = Year(now) & Month(now) & Day(now) & RanNum & "." & FileExt 
            TempStr = DateTime.Now.ToString("MMddHHmm") + RanNum + "." + FileExt;
            return TempStr;
        }


        /// <summary> 
        /// 检查上传目录，若无目录则自动建立 
        /// </summary> 
        /// <param name="PathValue">上传的目录（相对目录）</param> 
        /// <param name="bolPathCreatFlg">True：加上动态目录（年月） False:不加动态目录</param> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public static string CreatePath(string PathValue, bool bolPathCreatFlg)
        {
            // string functionReturnValue = null;
            string uploadpath = (bolPathCreatFlg ? DateTime.Now.ToString("yyyyMM") + "/" : "");
            string strPhysicalPath;// HttpContext.Current.Request.PhysicalApplicationPath;
            //if (!bolPathCreatFlg) uploadpath = "";

            if (!string.IsNullOrEmpty(PathValue))
            {
                if (!PathValue.EndsWith("/")) { PathValue = PathValue + "/"; }
            }

            strPhysicalPath = HttpContext.Current.Server.MapPath("/" + PathValue);
            //检查上传目录 

            if (Directory.Exists(strPhysicalPath + uploadpath) == false)
            {
                Directory.CreateDirectory(strPhysicalPath + uploadpath);
            }

            //if (bolPathCreatFlg)
            //{
            //    functionReturnValue = PathValue + uploadpath + "/";
            //}
            //else
            //{
            //    functionReturnValue = PathValue;
            //}
            return PathValue + uploadpath;
        }



        /// <summary> 
        /// 格式后缀 
        /// </summary> 
        /// <param name="UpFileExt"></param> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        private string FixName(string UpFileExt)
        {
            string strFixName;
            if (UpFileExt == null)
            {
                return "";
            }

            strFixName = UpFileExt.ToLower();
            // strFixName = strFixName.Replace(Strings.Chr(0), "");
            strFixName = strFixName.Replace(".", "");
            strFixName = strFixName.Replace(",", "");
            strFixName = strFixName.Replace("'", "");
            strFixName = strFixName.Replace("asp", "");
            strFixName = strFixName.Replace("asa", "");
            strFixName = strFixName.Replace("aspx", "");
            strFixName = strFixName.Replace("cer", "");
            strFixName = strFixName.Replace("cdx", "");
            strFixName = strFixName.Replace("htr", "");
            return strFixName.Replace("shtml", "");
        }

        /// <summary> 
        /// 判断文件类型是否合格 
        /// </summary> 
        /// <param name="FileExt"></param> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        private bool CheckFileExt(string FileExt)
        {
            //Dim Forumupload, i 

            if ((FileExt == null) || string.IsNullOrEmpty(FileExt))
            {
                return false;
            }

            if (FileExt == "asp" | FileExt == "asa" | FileExt == "aspx" | FileExt == "shtml")
            {
                return false;
            }
            strInceptFile = "," + strInceptFile + ",";
            if (strInceptFile.IndexOf("," + FileExt + ",") >= 0)
            {
                return true;
            }
            else
            {
                return false;

            }
        }


        /// <summary> 
        /// 上传处理过程 
        /// </summary> 
        /// <remarks></remarks> 
        public void SaveFile()
        {
            int iCount = 0;

            string FormName = "";
            FileInfo Objfile = default(FileInfo);
            string FileExt = null;
            string FileName = null;
            int FileType = 0;
            int iFileCount = 0;
            string FileNameForm;
            string FileNamePv = null;
            string strPhysicalPath = (!string.IsNullOrEmpty(UploadTruePath) ? UploadTruePath : HttpContext.Current.Server.MapPath("~/"));
            //"HW" '指定高宽缩放（可能变形） 
            // "W" '指定宽，高按比例 
            // "H" '指定高，宽按比例 
            // "Cut" '指定高宽裁减（不变形） 
            string[] strType = { "HW", "H", "W", "Cut" };
            // If Not IsEmpty(SessionName) Then 
            // If Session(SessionName) <> UploadObj.Form(SessionName) or Session(SessionName) = Empty Then 
            // ErrCodes = 7 
            // Exit Sub 
            // End If 
            // End If 
            iFileCount = FileArr.Length;

            if (iFileCount < 1)
            {
                iErrCodes = 3;
                return;
            }


            for (int i = 0; i <= iFileCount - 1; i++)
            {
                //列出所有上传文件 
                if (InceptMaxFile > 0 && iCount > InceptMaxFile)
                {
                    iErrCodes = 6;
                    return;
                }

                Objfile = new FileInfo(FileArr[i]);
                FileExt = FixName(System.IO.Path.GetExtension(System.IO.Path.GetFileName(Objfile.Name)));
                if (CheckFileExt(FileExt) == false)
                {
                    iErrCodes = 5;
                    return;
                }
                FileNameForm = System.IO.Path.GetFileName(Objfile.Name);
                FileName = (this.FileNameFlag > 0 ? (string.IsNullOrEmpty(FileName) ? System.IO.Path.GetFileName(Objfile.Name) : FileName) : FormatName(FileExt));
                FileNamePv = RNamePv + FileName;
                FileName = RName + FileName;

                FileType = CheckFiletype(FileExt);

                //FileNamePv = RName_PvStr & (IIf(RName_Str = "", FileName, FileName.Replace(RName_Str, ""))) 
                if (this.MaxSize > 0 && (Objfile.Length - iContentLength) > this.MaxSize)
                {
                    iErrCodes = 4;
                    return;
                }

                if (Objfile.Length > 0)
                {
                    //Objfile.SaveAs(strPhysicalPath & FilePath & FileName) 

                    //如果生成缩略图 
                    if (IsPreview == 1)
                    {
                        //图片的场合 
                        MakeThumbnail(Objfile.FullName, null, strPhysicalPath + UploadPvPath + FileNamePv, PreviewImageWidth, PreviewImageHeight, strType[(int)PreviewType], ScaleType);
                    }
                    Objfile.MoveTo(strPhysicalPath + UploadPath + FileName);

                    AddData(FormName, FileName, UploadPath, Objfile.Length - iContentLength, Objfile.Extension, FileType, FileNamePv, strFilePathPv, FileExt, null
                    , FileNameForm);
                    iCount = iCount + 1;
                    //CountSize = CountSize + Objfile.Length - iContentLength;
                }
                else
                {
                    iErrCodes = 8;
                }
                Objfile = null;

            }
        }

        /// <summary> 
        /// 上传处理过程 
        /// </summary> 
        /// <remarks></remarks> 
        public void SaveFileHttp()
        {
            int iCount = 0;
            bool isGifType = false;
            string FormName = "";
            HttpPostedFile Objfile = default(HttpPostedFile);
            string FileExt = null;
            string strFileName = null;
            int FileType = 0;
            // Flash2Jpeg ObjFlash2Jpg = default(Flash2Jpeg);
            int FileCount = 0;
            string FileNamePv = null;
            bool biType = false;
            int iType = 0;
            string FileNameForm;
            string strPhysicalPath = (!string.IsNullOrEmpty(UploadTruePath) ? UploadTruePath : HttpContext.Current.Server.MapPath("/" + strFilePath));
            string strPadimg = IsDeleteSImg ? "" : "src_";
            //"HW" '指定高宽缩放（可能变形） 
            // "W" '指定宽，高按比例 
            // "H" '指定高，宽按比例 
            // "Cut" '指定高宽裁减（不变形） 
            string[] strType = { "HW", "H", "W", "Cut", "Auto" };
            // If Not IsEmpty(SessionName) Then 
            // If Session(SessionName) <> UploadObj.Form(SessionName) or Session(SessionName) = Empty Then 
            // ErrCodes = 7 
            // Exit Sub 
            // End If 
            // End If 


            FileCount = lstUploadFileObj.Count;
            if (FileCount < 1)
            {
                iErrCodes = 3;
                return;
            }


            for (int i = 0; i <= lstUploadFileObj.Count - 1; i++)
            {
                //列出所有上传文件 
                if (iCount > InceptMaxFile)
                {
                    iErrCodes = 6;
                    return;
                }

                Objfile = lstUploadFileObj[i];
                if (Objfile.ContentLength <= 0)
                {
                    continue;
                }
                FileExt = FixName(System.IO.Path.GetExtension(System.IO.Path.GetFileName(Objfile.FileName)));
                if (CheckFileExt(FileExt) == false)
                {
                    iErrCodes = 5;
                    return;
                }

                if (this.MaxSize > 0 && (Objfile.ContentLength - iContentLength) > this.MaxSize)
                {
                    iErrCodes = 4;
                    return;
                }

                FileNameForm = System.IO.Path.GetFileName(Objfile.FileName);
                strFileName = (this.FileNameFlag > 0 ? (string.IsNullOrEmpty(FileName) ? System.IO.Path.GetFileName(Objfile.FileName) : (FileName + "." + FileExt)) : FormatName(FileExt));
                FileNamePv = RNamePv + strFileName;
                strFileName = RName + strFileName;
                FileType = CheckFiletype(FileExt);

                isGifType = (FileExt == "gif");// (FileExt == "gif" ? ImageAnimator.CanAnimate(Image.FromStream(Objfile.InputStream)) : false);

                //FileNamePv = RName_PvStr & (IIf(RName_Str = "", FileName, FileName.Replace(RName_Str, ""))) 

                if (Objfile.ContentLength > 0)
                {

                    if (FileType == 1)
                    {
                        iType = getImageType(Objfile.InputStream);
                        //int iWidth = 1024, iHeight = 768;
                        PreviewTypes iCutType = 0;

                        if (iType == 2)
                        {
                            iCutType = PreviewTypes.W;
                        }
                        else if (iType == 3)
                        {
                            iCutType = PreviewTypes.H;
                        }
                        else if (iType == 4 || iType == 1)
                        {
                            iCutType = PreviewTypes.Auto;
                        }

                        if (iType == 4)
                        {
                            Objfile.SaveAs(strPhysicalPath + strFileName);
                        }
                        else
                        {
                            if (!isGifType)
                            {
                                //压缩
                                if (MaxType != 3)
                                {
                                    MakeThumbnail("", Objfile.InputStream, strPhysicalPath + strPadimg + strFileName, 1024, 768, strType[(int)iCutType], ScaleType);
                                }
                                else //不压缩
                                {
                                    Objfile.SaveAs(strPhysicalPath + strFileName);
                                }

                                if (WaterMarkType != 0)
                                {
                                    MakeMark(strPhysicalPath + strPadimg + strFileName, Objfile.InputStream, strPhysicalPath + strFileName, HttpContext.Current.Server.MapPath("/") + "image/watermark.gif");
                                }

                            }
                            else
                            {
                                Objfile.SaveAs(strPhysicalPath + strFileName);
                                if (MaxType != 3)
                                {
                                    MakeThumbnail(strPhysicalPath + strFileName, null, strPhysicalPath + strPadimg + strFileName, 1024, 768, strType[(int)iCutType], ScaleType);
                                }

                                if (WaterMarkType != 0)
                                {
                                    MakeMark(strPhysicalPath + strPadimg + strFileName, Objfile.InputStream, strPhysicalPath + strFileName, HttpContext.Current.Server.MapPath("/") + "image/watermark.gif");
                                }
                            }
                        }


                        ////图片的场合 
                        //iType = getImageType(Objfile.InputStream);
                        ////图片大小类型 
                        //if (iType == 1)
                        //{

                        //    if (iMaxType == 1)
                        //    {
                        //        MakeThumbnail("", Objfile.InputStream, strPhysicalPath + "SRC_" + FileName, 1024, 768, strType[4]);
                        //    }
                        //    else if (FileExt.ToLower() == "gif")
                        //    {
                        //        Objfile.SaveAs(strPhysicalPath + FileName);
                        //    }
                        //    else
                        //    {
                        //        MakeThumbnail("", Objfile.InputStream, strPhysicalPath + "SRC_" + FileName, 1024, 768, strType[4]);
                        //    }

                        //    //正常图片 不缩放 
                        //    if (Draw_Info != null && !string.IsNullOrEmpty(Draw_Info))
                        //    {
                        //        MakeMark(strPhysicalPath + "SRC_" + FileName, Objfile.InputStream, strPhysicalPath + FileName, HttpContext.Current.Server.MapPath("/") + "image/main_05.gif", 0, 0, "");
                        //    }
                        //}
                        //else if (iType == 2)
                        //{

                        //    MakeThumbnail("", Objfile.InputStream, strPhysicalPath + "SRC_" + FileName, 1024, 768, strType[1]);
                        //    //大图片 等比缩放 
                        //    if (Draw_Info != null && !string.IsNullOrEmpty(Draw_Info))
                        //    {
                        //        MakeMark(strPhysicalPath + "SRC_" + FileName, Objfile.InputStream, strPhysicalPath + FileName, HttpContext.Current.Server.MapPath("/") + "image/main_05.gif", 0, 0, "");
                        //    }
                        //}
                        //else
                        //{
                        //    //小图片 不缩放 
                        //    if (FileExt.ToLower() == "gif")
                        //    {
                        //        //gif图片直接存储 不压缩 
                        //        Objfile.SaveAs(strPhysicalPath + FileName);
                        //    }
                        //    else
                        //    {
                        //        MakeThumbnail("", Objfile.InputStream, strPhysicalPath + FileName, 1024, 768, strType[4]);
                        //    }
                        //}

                        //如果生成缩略图 
                        if (IsPreview == 1)
                        {
                            if (FileType != 1)
                            {
                                //Flash的场合 
                                //ObjFlash2Jpg = new Flash2Jpeg();
                                //ObjFlash2Jpg.Flash2Jpeg(strPhysicalPath + FileName, View_ImageWidth, View_ImageHeight, strPhysicalPath + FileNamePv);
                            }
                            else
                            {
                                if (iType == 4)
                                {
                                    Objfile.SaveAs(strPhysicalPath + FileNamePv);
                                }
                                else
                                {
                                    if (isGifType)
                                    {
                                        // Objfile.SaveAs(strPhysicalPath + FileNamePv);
                                        MakeThumbnail(strPhysicalPath + FileNamePv, null, strPhysicalPath + FileNamePv, PreviewImageWidth, PreviewImageHeight, strType[(int)PreviewType], ScaleType);

                                    }
                                    else
                                    {
                                        //图片的场合 
                                        MakeThumbnail("", Objfile.InputStream, strPhysicalPath + FileNamePv, PreviewImageWidth, PreviewImageHeight, strType[(int)PreviewType], ScaleType);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Objfile.SaveAs(strPhysicalPath + strFileName);
                    }

                    AddData(FormName, strFileName, strFilePath, Objfile.ContentLength - iContentLength, Objfile.ContentType, FileType, FileNamePv, strFilePathPv, FileExt, Objfile
                    , FileNameForm);
                    iCount = iCount + 1;
                    //iCountSize = iCountSize + Objfile.ContentLength - ContentLength;
                }
                else
                {
                    iErrCodes = 8;
                }
                Objfile = null;

            }
        }

        /// <summary>
        /// 保存文件信息
        /// </summary>
        /// <param name="Form_Name"></param>
        /// <param name="File_Name"></param>
        /// <param name="File_Path"></param>
        /// <param name="File_Size"></param>
        /// <param name="File_ContentType"></param>
        /// <param name="File_Type"></param>
        /// <param name="FileNamePv"></param>
        /// <param name="FilePathPv"></param>
        /// <param name="File_Ext"></param>
        /// <param name="File_Data"></param>
        /// <param name="FileNameForm"></param>
        private void AddData(string Form_Name, string File_Name,
            string File_Path, long File_Size, string File_ContentType,
            int File_Type, string FileNamePv, string FilePathPv,
            string File_Ext, HttpPostedFile File_Data, string FileNameForm)
        {
            FileInfoStruc objFileInfo = new FileInfoStruc();
            objFileInfo.FormName = Form_Name;
            objFileInfo.FileName = File_Name;
            objFileInfo.FilePath = File_Path;
            objFileInfo.FileSize = File_Size;
            objFileInfo.FileType = File_Type;
            objFileInfo.FileNamePv = FileNamePv;
            objFileInfo.FilePathPv = FilePathPv;
            objFileInfo.FileContentType = File_ContentType;
            objFileInfo.FileExt = File_Ext;
            objFileInfo.FileData = File_Data;
            objFileInfo.FileNameForm = FileNameForm;
            //FileInfo.FileHeight = File_Height 
            //FileInfo.FileWidth = File_Width 
            lstUploadFiles.Add(objFileInfo);
            // FileInfo = null;
        }

        /// <summary> 
        /// 
        /// </summary> 
        /// <param name="oriImageStream"></param> 
        /// <returns>1：正常图片 2：超大图片 3：超小图片</returns> 
        /// <remarks></remarks> 
        public int getImageType(System.IO.Stream oriImageStream)
        {
            Image originalImage = Image.FromStream(oriImageStream);
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            int iRet = 0;

            if (ow > 1024)
            {
                return 2;
            }

            if (oh > 768)
            {
                return 3;
            }

            if (ow < 200 && oh < 200)
            {
                return 4;
            }
            originalImage.Dispose();

            return 1;
        }


        /// <summary> 
        /// 生成缩略图 
        /// </summary> 
        /// <param name="oriImagePath">源图路径（物理路径），和stream二选一</param> 
        /// <param name="oriImageStream">源图stream，和path二选一</param> 
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param> 
        /// <param name="width">缩略图宽度</param> 
        /// <param name="height">缩略图高度</param> 
        /// <param name="mode">生成缩略图的方式</param> 
        /// <param name="bolSaveFlag">是否保存到文件</param> 
        /// <remarks></remarks> 
        public static Image MakeThumbnail(string oriImagePath, System.IO.Stream oriImageStream,
            string thumbnailPath, int width, int height, string mode, int ScaleType = 0, bool bolSaveFlag = true)
        {
            LogManager.GetLogger("ImageUtility").Debug("Begin MakeThumbnail.....");
            string strExt = System.IO.Path.GetExtension(System.IO.Path.GetFileName(thumbnailPath));

            Image originalImage = default(Image);
            if (oriImagePath == null || string.IsNullOrEmpty(oriImagePath))
            {
                originalImage = Image.FromStream(oriImageStream);
            }
            else
            {
                originalImage = Image.FromFile(oriImagePath);
            }

            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            LogManager.GetLogger("ImageUtility").Debug(string.Format("originalImage: Width-{0}, Height-{1}", originalImage.Width, originalImage.Height));
            double iRate = 1;
            switch ((mode))
            {
                case "HW":
                    //指定高宽缩放（可能变形） 
                    break;
                case "W":
                    //指定宽，高按比例 
                    if (ScaleType == 0 || (ScaleType == 1 && originalImage.Width > width))
                    {
                        toheight = originalImage.Height*width/originalImage.Width;
                        iRate = (double) width/originalImage.Width;
                    }
                    else
                    {
                        towidth = ow;//变为原图宽
                        toheight = oh;//变为原图高
                    }
                    break;
                case "H":
                    //指定高，宽按比例 
                    if (ScaleType == 0 || (ScaleType == 2 && originalImage.Height > height))
                    {
                        towidth = originalImage.Width * height / originalImage.Height;
                        iRate = (double)height / originalImage.Height;
                    }
                    else
                    {
                        towidth = ow;//变为原图宽
                        toheight = oh;//变为原图高
                    }
                    break;
                case "Cut":
                    //指定高宽裁减（不变形） 

                    if ((((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)))
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;

                        iRate = (double)toheight / originalImage.Height;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                        iRate = (double)towidth / originalImage.Width;
                    }

                    break;
                case "Auto":
                    towidth = ow;
                    toheight = oh;
                    break;
                default:

                    break;
            }

            LogManager.GetLogger("ImageUtility").Debug(string.Format("thumbImage: Width-{0}, Height-{1}", towidth, toheight));

            //if (strExt.ToLower() == ".gif")
            //{
            //    originalImage.Dispose();

            //    //Stream gifStream;
            //    //if (oriImagePath == null || string.IsNullOrEmpty(oriImagePath))
            //    //{
            //    //    gifStream = oriImageStream;
            //    //}
            //    //else
            //    //{
            //    //    gifStream = new FileStream(oriImagePath, FileMode.Open);
            //    //}

            //    if (mode != "Cut")
            //    {
            //        GifHelper.GetThumbnail(oriImagePath, (double)iRate, thumbnailPath);
            //    }
            //    else
            //    {
            //        GifHelper.GetThumbnail(oriImagePath, thumbnailPath, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), iRate);
            //    }


            //    return;

            //  }



            //新建一个bmp图片 
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            //新建一个画板 
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);
            try
            {

                ImageCodecInfo[] icis = ImageCodecInfo.GetImageEncoders();
                //获取系统编码类型数组,包含了jpeg,bmp,png,gif,tiff 
                ImageCodecInfo ici = default(ImageCodecInfo);
                string strMimeType;

                switch (strExt.ToLower())
                {

                    case ".jpg":
                        strMimeType = "image/jpeg";
                        break;
                    case ".gif":
                        strMimeType = "image/gif";
                        break;
                    case ".png":
                        strMimeType = "image/png";
                        break;
                    case ".tif":
                        strMimeType = "image/tiff";
                        break;
                    case ".bmp":
                        strMimeType = "image/bmp";
                        break;
                    default:
                        strMimeType = "image/jpeg";
                        break;

                }

                foreach (ImageCodecInfo info in icis)
                {
                    if (info.MimeType == strMimeType)
                    {
                        ici = info;
                    }
                }

                EncoderParameters ep = new EncoderParameters(1);
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)80);
                //以jpg格式保存缩略图 

                if (bolSaveFlag)
                {
                    LogManager.GetLogger("ImageUtility").Debug("Saving file....");
                    if (ici != null)
                    {
                        bitmap.Save(thumbnailPath, ici, ep);
                    }
                    else
                    {
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    return bitmap;
                }
                else
                {
                    return bitmap;
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }

            return null;
        }

        public string MakeMark(string oriImagePath, System.IO.Stream oriImageStream, string savePath, string thumbnailPath)
        {

            //ImageWaterMark wm = new ImageWaterMark();

            ////原图，保存路径
            //wm.SourceImagePath = oriImagePath;
            //wm.SaveWaterMarkImagePath = savePath;

            //if (WaterMarkType == 1 || WaterMarkType == 3)
            //{
            //    //水印图片
            //    wm.WaterMarkImagePath = thumbnailPath;
            //    wm.WaterMarkImgAlign = WaterMarkImgAlign;
            //    wm.WaterMarkImgTransparence = WaterMarkImgTransparence;
            //    wm.WaterMarkImgAngle = WaterMarkImgAngle;
            //    wm.WaterMarkImgPlaceX = WaterMarkImgPlaceX;
            //    wm.WaterMarkImgPlaceY = WaterMarkImgPlaceY;
            //    wm.WaterMarkImageGroundColor = WaterMarkImageGroundColor;
            //}

            //if (WaterMarkType == 2 || WaterMarkType == 3)
            //{
            //    //wm.ConverImageEffect = ConvertEffect.Negative;
            //    wm.WaterMarkText = WaterMarkText;
            //    wm.WaterMarkTextTransparence = WaterMarkTextTransparence;
            //    wm.WaterMarkTextColor = WaterMarkTextColor;
            //    wm.WaterMarkTextAlign = WaterMarkTextAlign;
            //    wm.WaterMarkTextAngle = WaterMarkTextAngle;
            //    wm.WaterMarkTextCSS = WaterMarkTextCSS;
            //    wm.WaterMarkTextFont = WaterMarkTextFont;
            //    wm.WaterMarkTextPlaceX = WaterMarkTextPlaceX;
            //    wm.WaterMarkTextPlaceY = WaterMarkTextPlaceY;
            //    wm.WaterMarkTextSize = WaterMarkTextSize;
            //}
            ////wm.WaterMarkTextColor = "";
            //string strExt = System.IO.Path.GetExtension(System.IO.Path.GetFileName(oriImagePath));
            //if (strExt.ToLower() == ".gif")
            //{
            //    GifHelper.WaterMark(wm);
            //    // wm.GetToWaterMarkImage();
            //}
            //else
            //{
            //    wm.GetToWaterMarkImage();
            //}

            return "";
        }

        public static Image CreateWaterMarkImage(string text)
        {
            Bitmap bitmap = new Bitmap(320, 320);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Transparent);
            Font font = new Font("Arial", 36);
            Brush brush = new SolidBrush(Color.FromArgb(215, 215, 215, 215));
            PointF pos = new PointF();
            RectangleF layoutRectangle = new RectangleF(0, 0, 320, 320);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            DrawString(g, text, font, brush, layoutRectangle, stringFormat, -45F);
            g.Save();
            g.Dispose();

            return bitmap;
        }

        /// <summary>  
        /// 绘制根据矩形旋转文本  
        /// </summary>  
        /// <param name="graphics">Graphics</param>  
        /// <param name="s">文本</param>  
        /// <param name="font">字体</param>  
        /// <param name="brush">填充</param>  
        /// <param name="layoutRectangle">局部矩形</param>  
        /// <param name="format">布局方式</param>  
        /// <param name="angle">角度</param>  
        private static void DrawString(Graphics graphics, string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format, float angle)
        {
            // 求取字符串大小  
            SizeF size = graphics.MeasureString(s, font);

            // 根据旋转角度，求取旋转后字符串大小  
            SizeF sizeRotate = ConvertSize(size, angle);

            // 根据旋转后尺寸、布局矩形、布局方式计算文本旋转点  
            PointF rotatePt = GetRotatePoint(sizeRotate, layoutRectangle, format);

            // 重设布局方式都为Center  
            StringFormat newFormat = new StringFormat(format);
            newFormat.Alignment = StringAlignment.Center;
            newFormat.LineAlignment = StringAlignment.Center;

            // 绘制旋转后文本  
            DrawString(graphics, s, font, brush, rotatePt, newFormat, angle);
        }

        /// <summary>  
        /// 绘制根据点旋转文本，一般旋转点给定位文本包围盒中心点  
        /// </summary>  
        /// <param name="graphics">Graphics</param>  
        /// <param name="s">文本</param>  
        /// <param name="font">字体</param>  
        /// <param name="brush">填充</param>  
        /// <param name="point">旋转点</param>  
        /// <param name="format">布局方式</param>  
        /// <param name="angle">角度</param>  
        public static void DrawString(Graphics graphics, string s, Font font, Brush brush, PointF point, StringFormat format, float angle)
        {
            // Save the matrix  
            Matrix mtxSave = graphics.Transform;

            Matrix mtxRotate = graphics.Transform;
            mtxRotate.RotateAt(angle, point);
            graphics.Transform = mtxRotate;

            graphics.DrawString(s, font, brush, point, format);

            // Reset the matrix  
            graphics.Transform = mtxSave;
        }

        private static SizeF ConvertSize(SizeF size, float angle)
        {
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);

            // 旋转矩形四个顶点  
            PointF[] pts = new PointF[4];
            pts[0].X = -size.Width / 2f;
            pts[0].Y = -size.Height / 2f;
            pts[1].X = -size.Width / 2f;
            pts[1].Y = size.Height / 2f;
            pts[2].X = size.Width / 2f;
            pts[2].Y = size.Height / 2f;
            pts[3].X = size.Width / 2f;
            pts[3].Y = -size.Height / 2f;
            matrix.TransformPoints(pts);

            // 求取四个顶点的包围盒  
            float left = float.MaxValue;
            float right = float.MinValue;
            float top = float.MaxValue;
            float bottom = float.MinValue;

            foreach (PointF pt in pts)
            {
                // 求取并集  
                if (pt.X < left)
                    left = pt.X;
                if (pt.X > right)
                    right = pt.X;
                if (pt.Y < top)
                    top = pt.Y;
                if (pt.Y > bottom)
                    bottom = pt.Y;
            }

            SizeF result = new SizeF(right - left, bottom - top);
            return result;
        }

        private static PointF GetRotatePoint(SizeF size, RectangleF layoutRectangle, StringFormat format)
        {
            PointF pt = new PointF();

            switch (format.Alignment)
            {
                case StringAlignment.Near:
                    pt.X = layoutRectangle.Left + size.Width / 2f;
                    break;
                case StringAlignment.Center:
                    pt.X = (layoutRectangle.Left + layoutRectangle.Right) / 2f;
                    break;
                case StringAlignment.Far:
                    pt.X = layoutRectangle.Right - size.Width / 2f;
                    break;
                default:
                    break;
            }

            switch (format.LineAlignment)
            {
                case StringAlignment.Near:
                    pt.Y = layoutRectangle.Top + size.Height / 2f;
                    break;
                case StringAlignment.Center:
                    pt.Y = (layoutRectangle.Top + layoutRectangle.Bottom) / 2f;
                    break;
                case StringAlignment.Far:
                    pt.Y = layoutRectangle.Bottom - size.Height / 2f;
                    break;
                default:
                    break;
            }

            return pt;
        }

        /// <summary>
        ///  加水印文字
        /// </summary>
        /// <param name="image">imge 对象</param>
        /// <param name="_watermarkText">水印文字内容</param>
        /// <param name="_watermarkPosition">水印位置</param>
        public static Image AddWatermarkText(Image image, string _watermarkText, ImageAlign _watermarkPosition)
        {
            Image newImage = new Bitmap(image);
            Graphics picture = Graphics.FromImage(newImage);
            Font crFont = null;
            SizeF crSize = new SizeF();
            crFont = new Font("arial", 16, FontStyle.Bold);
            crSize = picture.MeasureString(_watermarkText, crFont);
            Size picSize = image.Size;

            float xpos = 0;
            float ypos = 0;

            switch (_watermarkPosition)
            {
                case ImageAlign.LeftTop:
                    xpos = ((float)crSize.Width * (float).01) + (crSize.Width / 2);
                    ypos = (float)crSize.Height * (float).01;
                    break;
                case ImageAlign.RightTop:
                    xpos = picSize.Width - (((float)crSize.Width * (float).99) - (crSize.Width / 2));
                    ypos = (float)crSize.Height * (float).01;
                    break;
                case ImageAlign.RightBottom:
                    xpos = picSize.Width - (((float)crSize.Width * (float).99) - (crSize.Width / 2));
                    ypos = picSize.Height - ((float)crSize.Height * (float).99) - crSize.Height / 2;
                    break;
                case ImageAlign.LeftBottom:
                    xpos = ((float)crSize.Width * (float).01) + (crSize.Width / 2);
                    ypos = picSize.Height - ((float)crSize.Height * (float).99) - crSize.Height / 2;
                    break;
            }

            StringFormat StrFormat = new StringFormat();
            StrFormat.Alignment = StringAlignment.Center;

            //SolidBrush semiTransBrush2 = new SolidBrush(Color.FromArgb(205, 205, 205, 205));
            //picture.DrawString(_watermarkText, crFont, semiTransBrush2, xpos + 1, ypos + 1, StrFormat);

            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(235, 235, 235, 235));
            picture.DrawString(_watermarkText, crFont, semiTransBrush, xpos, ypos, StrFormat);

            //semiTransBrush2.Dispose();
            semiTransBrush.Dispose();

            picture.Save();
            picture.Dispose();

            return newImage;
        }

    }

    public struct FileInfoStruc
    {
        public string FormName;
        public string FileName;
        public string FilePath;
        public string FileExt;
        public string FileNamePv;
        public string FilePathPv;
        public string FileNameForm;
        public string FileContentType;
        public long FileSize;
        public int FileType;
        public int FileWidth;
        public int FileHeight;
        public System.Web.HttpPostedFile FileData;
    }

    public enum PreviewTypes
    {
        /// <summary>
        /// 不缩放
        /// </summary>
        Auto = 4,
        /// <summary>
        /// 指定高宽缩放（变形）
        /// </summary>
        HW = 0,
        /// <summary>
        /// 指定高，宽按比例
        /// </summary>
        H = 1,
        /// <summary>
        /// 指定宽，高按比例
        /// </summary>
        W = 2,
        /// <summary>
        /// 指定宽高裁剪缩放（不变形），默认值
        /// </summary>
        Cut = 3
    }
    public enum ImageAlign : byte
    {
        Center = 4,
        CenterBottom = 7,
        CenterLeft = 3,
        CenterRight = 5,
        CenterTop = 1,
        LeftBottom = 6,
        LeftTop = 0,
        RightBottom = 8,
        RightTop = 2
    }

    public enum ShadowAlign : byte
    {
        LeftBottomShadow = 1,
        LeftTopShadow = 0,
        None = 4,
        RightBottomShadow = 3,
        RightTopShadow = 2
    }

    public enum TextCSS : byte
    {
        Bold = 0,
        Italic = 1,
        Strikeout = 3,
        Underline = 2
    }
}
