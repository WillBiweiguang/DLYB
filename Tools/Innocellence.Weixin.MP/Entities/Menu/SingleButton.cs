﻿/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：SingleButton.cs
    文件功能描述：所有单击按钮的基类
    
    
    创建标识：Innocellence - 20150211
    
    修改标识：Innocellence - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innocellence.Weixin.MP.Entities.Menu
{
    /// <summary>
    /// 所有单击按钮的基类（view，click等）
    /// </summary>
    public abstract class SingleButton : BaseButton, IBaseButton
    {
        /// <summary>
        /// 按钮类型（click或view）
        /// </summary>
        public string type { get; set; }

        public SingleButton(string theType)
        {
            type = theType;
        }
    }
}
