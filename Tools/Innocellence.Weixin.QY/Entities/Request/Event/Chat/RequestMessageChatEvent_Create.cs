/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：RequestMessageChatEvent_Create.cs
    文件功能描述：创建会话
    
    
    创建标识：DLYB - 20160313
    
    修改标识：DLYB - 20160313
    修改描述：整理接口
----------------------------------------------------------------*/

namespace DLYB.Weixin.QY.Entities
{
    /// <summary>
    /// 创建会话
    /// </summary>
    public class RequestMessageChatEvent_Create : RequestMessageEventBase, IRequestMessageEventBase
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override Event Event
        {
            get { return Event.create_chat; }
        }


        /// <summary>
        /// 会话信息
        /// </summary>
        public ChatInfo ChatInfo { get; set; }

    }
}
