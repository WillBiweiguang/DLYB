/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：RequestMessageText.cs
    文件功能描述：接收普通文本消息
    
    
    创建标识：DLYB - 20150313
    
    修改标识：DLYB - 20150313
    修改描述：整理接口
----------------------------------------------------------------*/

using DLYB.Weixin.Entities;
using System.Collections.Generic;
namespace DLYB.Weixin.QY.Entities
{
    public class RequestMessageChat : MessageBase, IRequestMessageBase
    {
        public virtual RequestMsgType MsgType
        {
            get { return RequestMsgType.Chat; }
        }

        /// <summary>
        /// 企业应用的id，整型。可在应用的设置页面查看
        /// </summary>
        public string AgentType { get; set; }

        public int ItemCount { get; set; }
        public string PackageId { get; set; }

        public List<IRequestMessageBase> Item { get; set; }


      public  int AgentID { get; set; }
      public long MsgId { get; set; }



    }
}
