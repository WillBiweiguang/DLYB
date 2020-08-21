/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：RequestMessageChatEvent_Quit.cs
    文件功能描述：退出会话
    
    
    创建标识：DLYB - 20160313
    
    修改标识：DLYB - 20160313
    修改描述：整理接口
----------------------------------------------------------------*/

namespace DLYB.Weixin.QY.Entities
{
    /// <summary>
    /// 退出会话
    /// </summary>
    public class RequestMessageChatEvent_Quit : RequestMessageEventBase, IRequestMessageEventBase
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override Event Event
        {
            get { return Event.quit_chat; }
        }


        /// <summary>
        /// 会话id
        /// </summary>
        public string ChatId { get; set; }

    }
}
