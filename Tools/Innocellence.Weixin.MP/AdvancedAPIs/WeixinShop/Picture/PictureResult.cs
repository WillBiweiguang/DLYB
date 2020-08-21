using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innocellence.Weixin.MP.Entities;

namespace Innocellence.Weixin.MP.AdvancedAPIs
{
    /// <summary>
    /// 上传图片返回结果
    /// </summary>
    public class PictureResult : WxJsonResult
    {
        public string image_url { get; set; }//图片Url
    }
}

