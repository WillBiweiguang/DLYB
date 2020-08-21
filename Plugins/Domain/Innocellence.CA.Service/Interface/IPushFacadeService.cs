using Infrastructure.Core;
using DLYB.CA.Contracts.CommonEntity;

namespace DLYB.CA.Service.Interface
{
    public interface IPushFacadeService : IDependency
    {
        Result<CheckResult> Verify(ConfigedInfo appInfo, object entity, ContentType contentType);
    }
}
