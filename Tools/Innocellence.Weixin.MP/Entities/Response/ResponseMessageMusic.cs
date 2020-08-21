/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：ResponseMessageMusic.cs
    文件功能描述：响应回复音乐消息
    
    
    创建标识：DLYB - 20150211
    
    修改标识：DLYB - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

namespace DLYB.Weixin.MP.Entities
{
    public class ResponseMessageMusic : ResponseMessageBase, IResponseMessageBase
    {
        public override ResponseMsgType MsgType
        {
            get { return ResponseMsgType.Music; }
        }

        
    }
}
