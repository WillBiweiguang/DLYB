/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：RequestMessageText.cs
    文件功能描述：接收普通文本消息
    
    
    创建标识：DLYB - 20150313
    
    修改标识：DLYB - 20150313
    修改描述：整理接口
----------------------------------------------------------------*/

namespace DLYB.Weixin.QY.Entities
{
    public class RequestMessageChatText : RequestMessageText
    {
        public Receiver Receiver { get; set; }
    }
}
