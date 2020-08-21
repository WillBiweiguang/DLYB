/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：RequestMessageEvent_Unsubscribe.cs
    文件功能描述：事件之取消关注事件的推送（unsubscribe）
    
    
    创建标识：DLYB - 20150313
    
    修改标识：DLYB - 20150313
    修改描述：整理接口
----------------------------------------------------------------*/

namespace DLYB.Weixin.QY.Entities
{
    /// <summary>
    /// 事件之取消关注事件的推送（unsubscribe）
    /// </summary>
    public class RequestMessageEvent_UnSubscribe : RequestMessageEventBase, IRequestMessageEventBase
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override Event Event
        {
            get { return Event.unsubscribe; }
        }
    }
}
