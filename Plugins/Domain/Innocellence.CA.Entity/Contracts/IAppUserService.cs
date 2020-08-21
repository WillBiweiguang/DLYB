using System.Collections.Generic;
using Infrastructure.Core;
using Infrastructure.Utility.Data;
using DLYB.CA.Contracts.ViewModel;

namespace DLYB.CA.Contracts.Contracts
{
    public interface IAppUserService : IDependency
    {
        IList<AppUserView> QueryUser(string searchKey, PageCondition pageCondition);
    }
}
