// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 17:21</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Core;
using Infrastructure.Core.Data;
using DLYB.CA.Entity;
using DLYB.CA.Contracts;


namespace DLYB.CA.Services
{
    /// <summary>
    /// 业务实现——身份认证模块
    /// </summary>
    public partial class WeChatAppUserService : BaseService<WeChatAppUser>, IWeChatAppUserService
    {
        public WeChatAppUserService()
            : base("CAAdmin")
        {

        }


    }
}