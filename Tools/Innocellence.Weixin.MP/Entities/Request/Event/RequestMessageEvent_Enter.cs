/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：RequestMessageEvent_Enter.cs
    文件功能描述：事件之进入会话
    
    
    创建标识：DLYB - 20150211
    
    修改标识：DLYB - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

namespace DLYB.Weixin.MP.Entities
{
    public class RequestMessageEvent_Enter : RequestMessageEventBase, IRequestMessageEventBase
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override Event Event
        {
            get { return Event.ENTER; }
        }
    }
}
