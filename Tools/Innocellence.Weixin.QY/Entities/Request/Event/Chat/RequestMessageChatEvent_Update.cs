/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：RequestMessageChatEvent_Update.cs
    文件功能描述：修改会话
    
    
    创建标识：DLYB - 20160313
    
    修改标识：DLYB - 20160313
    修改描述：整理接口
----------------------------------------------------------------*/

namespace DLYB.Weixin.QY.Entities
{
    /// <summary>
    /// 修改会话
    /// </summary>
    public class RequestMessageChatEvent_Update : RequestMessageEventBase, IRequestMessageEventBase
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override Event Event
        {
            get { return Event.update_chat; }
        }


        /// <summary>
        /// 	会话名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 会话所有者（管理员）
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// 会话新增成员列表，成员间以竖线分隔，如：zhangsan|lisi
        /// </summary>
        public string AddUserList { get; set; }

        /// <summary>
        /// 会话删除成员列表，成员间以竖线分隔，如：zhangsan|lisi
        /// </summary>
        public string DelUserList { get; set; }

        /// <summary>
        /// 会话id
        /// </summary>
        public string ChatId { get; set; }

    }
}
