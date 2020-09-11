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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;


using System.Data.Entity.Infrastructure;
using System.Globalization;

using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using Infrastructure.Web.Domain.Service.Common;
using System.Security.Principal;
using Infrastructure.Web.Domain.ModelsView;



namespace Infrastructure.Web.Domain.Services
{
    /// <summary>
    /// 
    /// </summary>
    public partial class HistoricalCostService :  BaseService<HistoricalCost>, IHistoricalCostService
    {
        public List<T> GetListByProjectID<T>(int projectId) where T : IViewModel, new()
        {

            var ens = Repository.Entities.Where(a => a.ProjectId == projectId).OrderBy(a => a.CreatedDate).ToList();

            var lst = ens.Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
        }
    }
}