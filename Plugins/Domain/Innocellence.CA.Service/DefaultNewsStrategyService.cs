using System;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Service.Interface;

namespace DLYB.CA.Service
{
    public class DefaultNewsStrategyService : IPushStrategyService
    {
        public Result<CheckResult> CheckPushRule(object entity, ConfigedInfo appInfo)
        {
            throw new NotImplementedException();
        }

        public ContentType ContentType
        {
            get
            {
                return ContentType.News;
            }
        }
    }
}
