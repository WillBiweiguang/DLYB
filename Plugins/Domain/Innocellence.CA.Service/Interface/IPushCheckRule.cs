using System;
using Infrastructure.Core;
using DLYB.CA.Contracts.CommonEntity;

namespace DLYB.CA.Service.Interface
{
    public interface IPushCheckRule : IDependency
    {
        Result<CheckResult> Verify(ConfigedInfo appInfo, TargetInfoObject targetInfo, Result<CheckResult> result);
    }

    public class BasePushCheckRule
    {
        protected virtual void Init(Result<CheckResult> result)
        {
            if (result==null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.Data == null)
            {
                result.Data = new CheckResult();
            }

            if (result.Data.Success == null)
            {
                result.Data.Success = new SuccessResult();
            }

            if (result.Data.Error == null)
            {
                result.Data.Error = new ErrorResult();
            }
        }
    }
}
