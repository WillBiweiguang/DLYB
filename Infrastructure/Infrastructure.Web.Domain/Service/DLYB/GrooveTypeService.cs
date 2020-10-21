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
    public partial class GrooveTypeService : BaseService<GrooveTypes>, IGrooveTypeService
    {
        public List<GrooveTypeView> GetGrooveTypeQuerys()
        {
            return Repository.Entities.Where(y => y.IsDeleted != true).Select(x => new GrooveTypeView()
            {
                WeldGeometry = x.WeldGeometry,
                GrooveType = x.GrooveType,
                WorksThickness1 = x.WorksThickness1,
                WorksThickness2 = x.WorksThickness2,
                PreviewImage = x.PreviewImage,
                WorksThicknessH1 = x.WorksThicknessH1,
                WorksThicknessH2 = x.WorksThicknessH2,
                GrooveClearance = x.GrooveClearance,
                BluntThickness = x.BluntThickness,
                GrooveAngleA1 = x.GrooveAngleA1,
                GrooveAngleA2 = x.GrooveAngleA2,
                GrooveArcR1 = x.GrooveArcR1,
                GrooveArcR2 = x.GrooveArcR2,
                CircleArcR = x.CircleArcR,
                GrooveThicknessT = x.GrooveThicknessT,
                WorksThicknessH3 = x.WorksThicknessH3,
                WeldLeg1 = x.WeldLeg1,
            }).Distinct().ToList();
        }
    }
}