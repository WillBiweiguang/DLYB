/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：RequestMessageVoice.cs
    文件功能描述：接收普通语音消息
    
    
    创建标识：DLYB - 20150313
    
    修改标识：DLYB - 20150313
    修改描述：整理接口
----------------------------------------------------------------*/

namespace DLYB.Weixin.QY.Entities
{
    public class RequestMessageVoice : RequestMessageBase,IRequestMessageBase
    {
        public override RequestMsgType MsgType
        {
            get { return RequestMsgType.Voice; }
        }

        /// <summary>
        /// 语音消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 语音格式：amr
        /// </summary>
        public string Format { get; set; }
    }
}
