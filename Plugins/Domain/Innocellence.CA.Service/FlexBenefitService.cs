using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using DLYB.CA.Contracts;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace DLYB.CA.Services
{
    public partial class FlexBenefitService : BaseService<FlexBenefit>, IFlexBenefitService
    {
        public FlexBenefitService()
            : base("CAAdmin")
        {

        }

        public FlexBenefit GetFlexBenefitByConditions(string lillyID, string accessYear)
        {
            //FlexBenefit flexBenefit = new FlexBenefit();
            if (string.IsNullOrEmpty(lillyID) || string.IsNullOrEmpty(accessYear))
            {
                return null;
            }

            return Repository.Entities.Where(a => lillyID.Equals(a.LillyID) && accessYear.Equals(a.AccessYear)).Distinct().FirstOrDefault();
            //return flexBenefit;
        }
    }
}