/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：RequestMessageShortVideo.cs
    文件功能描述：接收小视频消息
    
    
    创建标识：DLYB - 20150708
----------------------------------------------------------------*/

namespace DLYB.Weixin.QY.Entities
{
    public class RequestMessageChatShortVideo : RequestMessageShortVideo
    {
        public Receiver Receiver { get; set; }
    }
}
