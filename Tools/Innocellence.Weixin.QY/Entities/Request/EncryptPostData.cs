/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：EncryptPostData.cs
    文件功能描述：原始加密信息
    
    
    创建标识：DLYB - 20150313
----------------------------------------------------------------*/

namespace DLYB.Weixin.QY.Entities
{
    public class EncryptPostData
    {
        public string ToUserName { get; set; }
        public string Encrypt { get; set; }
        public int AgentID { get; set; }
    }
}
