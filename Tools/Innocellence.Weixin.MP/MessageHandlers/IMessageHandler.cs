/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：IMessageHandler.cs
    文件功能描述：MessageHandler接口
    
    
    创建标识：DLYB - 20150924
    
----------------------------------------------------------------*/

using DLYB.Weixin.MessageHandlers;
using DLYB.Weixin.MP.Entities;

namespace DLYB.Weixin.MP.MessageHandlers
{

    public interface IMessageHandler : IMessageHandler<IRequestMessageBase, IResponseMessageBase>
    {
        new IRequestMessageBase RequestMessage { get; set; }
        new IResponseMessageBase ResponseMessage { get; set; }
    }
}
