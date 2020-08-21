using Infrastructure.Core;
using Infrastructure.Core.Data;
using DLYB.CA.Contracts;
using DLYB.CA.Contracts.ViewModel;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DLYB.CA.Service
{
    public class UserInfoService : BaseService<UserInfo>, IUserInfoService
    {
        public UserInfoService()
           : base("CAAdmin")
        {


        }

       
        public UserInfo GetBylillyId(string LillyId)
        {

            Expression<Func<UserInfo, bool>> predicate = n => n.LillyId == LillyId;
            var t = Repository.Entities.Where(predicate).ToList().FirstOrDefault();
           
            return t;
        }


    }
}
