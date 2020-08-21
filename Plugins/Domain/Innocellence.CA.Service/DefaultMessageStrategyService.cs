using System.Collections.Generic;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Service.Interface;

namespace DLYB.CA.Service
{
    public class DefaultMessageStrategyService : BaseStrategyService, IPushStrategyService
    {
        private readonly IList<IPushCheckRule> _rules = new List<IPushCheckRule> { new DefaultUserRule(), new DefaultDepartmentRule(),new DefaultTagRule() };

        public Result<CheckResult> CheckPushRule(object entity, ConfigedInfo appInfo)
        {
            var result = new Result<CheckResult>();

            foreach (var rule in _rules)
            {
                rule.Verify(appInfo, (TargetInfoObject)entity, result);
            }

            //base.HandlerResult(result, appInfo, entity);

            return result;
        }

        public ContentType ContentType
        {
            get { return ContentType.Message; }
        }
    }

    public class MessageTextStrategyService : BaseStrategyService, IPushStrategyService
    {
        private readonly IList<IPushCheckRule> _rules = new List<IPushCheckRule> { new DefaultUserRule(), new DefaultDepartmentRule(), new DefaultTagRule() };

        public Result<CheckResult> CheckPushRule(object entity, ConfigedInfo appInfo)
        {
            var result = new Result<CheckResult>();

            foreach (var rule in _rules)
            {
                rule.Verify(appInfo, (TargetInfoObject)entity, result);
            }

            //base.HandlerResult(result, appInfo, entity);

            return result;
        }

        public ContentType ContentType
        {
            get { return ContentType.Text; }
        }
    }
}
