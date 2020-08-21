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
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Contracts;
using System.Linq.Expressions;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Service.Common;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Autofac;
using Infrastructure.Web.Domain.Service;
using Novell.Directory.Ldap.Utilclass;


namespace Infrastructure.Web.Domain.Services
{
    /// <summary>
    /// 业务实现——身份认证模块
    /// </summary>
    public partial class ReleaseNotesService : BaseService<ReleaseNote>, IReleaseNoteService
    {

    }
}