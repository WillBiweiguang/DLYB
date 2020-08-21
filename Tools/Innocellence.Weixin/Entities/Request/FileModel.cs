/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：EncryptPostModel.cs
    文件功能描述：加解密消息统一基类 接口
    
    
    创建标识：Innocellence - 20150211
    
    修改标识：Innocellence - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Innocellence.Weixin
{


    /// <summary>
    /// 接收加密信息统一基类
    /// </summary>
    public class FileModel 
    {
        string strFileName { set; get; }
        Stream FileStream { set; get; }
    }
}
