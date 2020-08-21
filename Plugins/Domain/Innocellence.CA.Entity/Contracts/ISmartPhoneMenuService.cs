using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;
using DLYB.CA.Contracts.ViewModel;
using DLYB.Weixin.Entities;

namespace DLYB.CA.Contracts.Contracts
{
    public interface ISmartPhoneMenuService : IDependency, IBaseService<Category>
    {
        QyJsonResult Push(int appId);

        AppMenuView QueryMenuViewById(int menuId);

        int UpdateOrAdd(AppMenuView view);
    }
}
