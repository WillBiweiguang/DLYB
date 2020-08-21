using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;
using System.Linq.Expressions;
using DLYB.CA.ViewModel;

namespace DLYB.CA.Contracts
{
    public interface IAccessDashboardService : IDependency, IBaseService<AccessDashboard>
    {
        AccessDashboardConditionView GetAccessDashboardConditions();

        List<AccessDashboardConditionView> GetAllAccessDashboards();

        List<T> GetSearchResult<T>(Expression<Func<AccessDashboard, bool>> predicate) where T : IViewModel, new();
    }
}
