/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：RequestMessageImage.cs
    文件功能描述：接收普通图片消息
    
    
    创建标识：DLYB - 20150313
    
    修改标识：DLYB - 20150313
    修改描述：整理接口
----------------------------------------------------------------*/

namespace DLYB.Weixin.QY.Entities
{
    public class RequestMessageChatImage : RequestMessageImage
    {

        public Receiver Receiver { get; set; }
      
    }
}
