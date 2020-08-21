/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：GetIdResult.cs
    文件功能描述：获取用户分组ID返回结果
    
    
    创建标识：DLYB - 20150211
    
    修改标识：DLYB - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using DLYB.Weixin.Entities;

namespace DLYB.Weixin.MP.AdvancedAPIs.Groups
{
    /// <summary>
    /// 获取用户分组ID返回结果
    /// </summary>
    public class GetGroupIdResult : WxJsonResult
    {
        public int groupid { get; set; }
    }
}
