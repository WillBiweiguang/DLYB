﻿/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：ResponseMessageText.cs
    文件功能描述：响应回复文本消息
    
    
    创建标识：DLYB - 20150211
    
    修改标识：DLYB - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

namespace DLYB.Weixin.MP.Entities
{
    public class ResponseMessageText : ResponseMessageBase, IResponseMessageBase
    {
        new public virtual ResponseMsgType MsgType
        {
            get { return ResponseMsgType.Text; }
        }

        public string Content { get; set; }
    }
}
