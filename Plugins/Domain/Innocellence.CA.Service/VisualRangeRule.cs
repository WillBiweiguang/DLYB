using System.Linq;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Service.Interface;
using CheckResult = DLYB.CA.Service.Interface.CheckResult;

namespace DLYB.CA.Service
{
    public class DefaultTagRule : BasePushCheckRule, IPushCheckRule
    {
        public Result<CheckResult> Verify(ConfigedInfo appInfo, TargetInfoObject targetInfo, Result<CheckResult> result)
        {
            base.Init(result);

            if (targetInfo.Tags == null || !targetInfo.Tags.Any())
            {
                if (result.Status == 0)
                {
                    result.Status = 200;
                }
                return result;
            }

            if (result.Status == 0)
            {
                result.Status = 200;
            }
            result.Data.Success.ToTags = targetInfo.Tags.ConvertTagIdToObject();
            return result;

            //if (!appInfo.AppInfoResult.allow_tags.tagid.Any())
            //{
            //    result.Status = 99;
            //    result.Data.Error.ErrorTags = targetInfo.Tags.ConvertTagIdToObject();
            //    return result;
            //}

            //var allAssignedTags = appInfo.AppInfoResult.allow_tags.tagid;

            //var errorTags = targetInfo.Tags.Where(x => !allAssignedTags.Any(y => x == y)).ToList();

            //if (errorTags.Any())
            //{
            //    result.Status = 99;
            //}
            //else
            //{
            //    if (result.Status == 0)
            //    {
            //        result.Status = 200;
            //    }

            //    result.Data.Success.ToTags = targetInfo.Tags.ConvertTagIdToObject();
            //    return result;
            //}

            //var successTags = targetInfo.Tags.Where(x => errorTags.Any(y => x == y)).ToList();

            //result.Data.Success.ToTags = successTags.ConvertTagIdToObject();

            //result.Data.Error.ErrorTags = errorTags.ConvertTagIdToObject();

            //return result;
        }
    }
}
