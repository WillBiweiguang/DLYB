using System;
using System.Collections.Generic;
using System.Linq;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Service.Common;
using DLYB.CA.Service.Interface;

namespace DLYB.CA.Service
{
    public class DefaultUserRule : BasePushCheckRule, IPushCheckRule
    {
        public Result<CheckResult> Verify(ConfigedInfo appInfo, TargetInfoObject targetInfo, Result<CheckResult> result)
        {
            base.Init(result);

            if (targetInfo.Users == null || !targetInfo.Users.Any())
            {
                if (result.Status == 0)
                {
                    result.Status = 200;
                }
                result.Data.Success.ToUsers = new List<string>();
                return result;
            }

            if (!appInfo.AssignedUserIds.Any())
            {
                result.Status = 99;
                result.Data.Error.ErrorUsers = targetInfo.Users;
                return result;
            }

            //including under tag
            var allAssignedUsers = appInfo.AssignedUserIds;

            var errorUsers = targetInfo.Users.Where(targetUser =>
            {
                var user = WeChatCommonService.lstUser.FirstOrDefault(x => string.Equals(x.userid, targetUser, StringComparison.CurrentCultureIgnoreCase));

                return
                    (allAssignedUsers.All(
                        assignedUser => string.Compare(assignedUser, targetUser, StringComparison.OrdinalIgnoreCase) != 0) ||
                     user == null ||
                     user.status == 4);

            }).ToList();

            if (errorUsers.Any())
            {
                result.Status = 99;
            }
            else
            {
                if (result.Status == 0)
                {
                    result.Status = 200;
                }

                result.Data.Success.ToUsers = targetInfo.Users;
                return result;
            }

            var successUsers = targetInfo.Users.Where(x => errorUsers.All(y => x.ToUpper() != y.ToUpper())).ToList();

            result.Data.Error.ErrorUsers = errorUsers;

            result.Data.Success.ToUsers = successUsers;

            return result;
        }
    }
}
