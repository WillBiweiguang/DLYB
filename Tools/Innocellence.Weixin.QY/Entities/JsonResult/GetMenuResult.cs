/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：GetMenuResult.cs
    文件功能描述：获取菜单返回的Json结果
    
    
    创建标识：DLYB - 20150313
    
    修改标识：DLYB - 20150313
    修改描述：整理接口
----------------------------------------------------------------*/

using DLYB.Weixin.QY.Entities.Menu;

namespace DLYB.Weixin.QY.Entities
{
    /// <summary>
    /// GetMenu返回的Json结果
    /// </summary>
    public class GetMenuResult
    {
        public ButtonGroup menu { get; set; }

        public GetMenuResult()
        {
            menu = new ButtonGroup();
        }
    }
}
