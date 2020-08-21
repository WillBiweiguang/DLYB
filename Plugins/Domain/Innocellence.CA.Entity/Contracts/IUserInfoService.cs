using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using DLYB.CA.Entity;
using DLYB.CA.ModelsView;
using DLYB.CA.Contracts.ViewModel;

namespace DLYB.CA.Contracts
{
    public interface IUserInfoService : IDependency, IBaseService<UserInfo>
    {
        UserInfo GetBylillyId(string LillyId);
       
    }
}
