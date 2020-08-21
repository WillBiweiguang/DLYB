/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
  
    文件名：NormalAppResult.cs
    文件功能描述：普通API返回类型
    
    
    创建标识：DLYB - 20150319
----------------------------------------------------------------*/

namespace DLYB.Weixin.MP.AppStore
{
    /// <summary>
    /// 普通API返回类型
    /// </summary>
    public class NormalAppResult : AppResult<NormalAppData>
    {

    }

    public class NormalAppData : IAppResultData
    {
        public object Object { get; set; }
    }
}
