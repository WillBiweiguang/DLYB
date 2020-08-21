// -----------------------------------------------------------------------
//  <copyright file="LogisticsPayment.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 20:57</last-date>
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace Infrastructure.Web.Net.Alipay
{
    /// <summary>
    /// 表示物流支付方式的枚举
    /// </summary>
    public enum LogisticsPayment
    {
        /// <summary>
        /// 买家承担运费
        /// </summary>
        BUYER_PAY,

        /// <summary>
        /// 卖家承担运费
        /// </summary>
        SELLER_PAY,

        ///// <summary>
        ///// 买家货到付款
        ///// </summary>
        //[Description("买家到货付款")]
        //BUYER_PAY_AFTER_RECEIVE
    }
}