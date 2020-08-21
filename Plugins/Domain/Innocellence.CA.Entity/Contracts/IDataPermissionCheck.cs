using Infrastructure.Core;

namespace DLYB.CA.Contracts.Contracts
{
    public interface IDataPermissionCheck : IDependency
    {
        bool AppDataCheck(int targetAppId, int currentAppId);
    }
}
