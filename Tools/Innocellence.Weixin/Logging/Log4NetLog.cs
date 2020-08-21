// -----------------------------------------------------------------------
//  <copyright file="Log4NetLog.cs" company="OSharp开源团队">
//      Copyright (c) 2014-2015 OSharp. All rights reserved.
//  </copyright>
//  <last-editor></last-editor>
//  <last-date>2015-02-08 15:51</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using log4net.Core;

using  log4net;


namespace DLYB.Weixin.Logging
{
    /// <summary>
    /// log4net 日志输出者适配类
    /// </summary>
    internal class Log4NetLog
    {

        public static readonly ILog Logger = log4net.LogManager.GetLogger("WeChat");
    }

}