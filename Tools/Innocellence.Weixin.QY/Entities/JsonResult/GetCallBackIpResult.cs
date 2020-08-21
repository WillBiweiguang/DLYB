/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：GetCallBackIpResult.cs
    文件功能描述：获取微信服务器的ip段的JSON返回格式
    
    
    创建标识：DLYB - 20150313
    
    修改标识：DLYB - 20150313
    修改描述：整理接口
----------------------------------------------------------------*/

using DLYB.Weixin.Entities;

namespace DLYB.Weixin.QY.Entities
{
    public class GetCallBackIpResult : QyJsonResult
    {
        public string[] ip_list { get; set; }
    }
}
