/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：IRequestMessageEventKey.cs
    文件功能描述：具有EventKey属性的RequestMessage接口
    
    
    创建标识：DLYB - 20150211
    
    修改标识：DLYB - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

namespace DLYB.Weixin.MP.Entities
{
    /// <summary>
    /// 具有EventKey属性的RequestMessage接口
    /// </summary>
    public interface IRequestMessageEventKey
    {
        string EventKey { get; set; }
    }
}
