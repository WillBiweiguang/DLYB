﻿/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：SingleViewButton.cs
    文件功能描述：Url按钮
    
    
    创建标识：DLYB - 20150313
    
    修改标识：DLYB - 20150313
    修改描述：整理接口
----------------------------------------------------------------*/

namespace DLYB.Weixin.QY.Entities.Menu
{
    /// <summary>
    /// Url按键
    /// </summary>
    public class SingleViewButton : SingleButton
    {
        /// <summary>
        /// 类型为view时必须
        /// 网页链接，用户点击按钮可打开链接，不超过256字节
        /// </summary>
        public string url { get; set; }

        public SingleViewButton()
            : base(ButtonType.view.ToString())
        {
        }
    }
}
