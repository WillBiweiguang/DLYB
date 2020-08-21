﻿/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：MessageHandlerException.cs
    文件功能描述：微信消息异常处理类
    
    
    创建标识：DLYB - 20150211
    
    修改标识：DLYB - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using System;

namespace DLYB.Weixin.Exceptions
{
    public class MessageHandlerException : WeixinException
    {
          public MessageHandlerException(string message)
            : base(message, null)
        {
        }

          public MessageHandlerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
