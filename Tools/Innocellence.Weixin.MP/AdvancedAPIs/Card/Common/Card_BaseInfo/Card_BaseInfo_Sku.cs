﻿/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB

    文件名：Card_BaseInfo_Sku.cs
    文件功能描述：基本的卡券商品信息，所有卡券通用。


    创建标识：DLYB - 20150211

    修改标识：DLYB - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

namespace DLYB.Weixin.MP.AdvancedAPIs.Card
{
    /// <summary>
    /// 商品信息
    /// </summary>
    public class Card_BaseInfo_Sku
    {
        /// <summary>
        /// 上架的数量。（不支持填写0或无限大）
        /// 必填
        /// </summary>
        public int quantity { get; set; }
    }
}
