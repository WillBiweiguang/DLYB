/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：ConvertToUserIdResult.cs
    文件功能描述：openid转换成userid接口返回的Json结果
    
    
    创建标识：DLYB - 20150722
----------------------------------------------------------------*/

using DLYB.Weixin.Entities;

namespace DLYB.Weixin.QY.Entities
{
    /// <summary>
    /// openid转换成userid接口返回的Json结果
    /// </summary>
    public class ConvertToUserIdResult : QyJsonResult
    {
        /// <summary>
        /// 该openid在企业号中对应的成员userid
        /// </summary>
        public string userid { get; set; }
    }
}
