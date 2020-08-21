using System.Collections.Generic;
using System.Linq;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Entity;
using DLYB.CA.Service.Common;
using DLYB.CA.Service.Interface;
using CheckResult = DLYB.CA.Service.Interface.CheckResult;

namespace DLYB.CA.Service
{
    public class DefaultDepartmentRule : BasePushCheckRule,IPushCheckRule
    {
        public Result<CheckResult> Verify(ConfigedInfo appInfo, TargetInfoObject targetInfo, Result<CheckResult> result)
        {
           base.Init(result);

            if (targetInfo.Departments == null || !targetInfo.Departments.Any())
            {
                if (result.Status == 0)
                {
                    result.Status = 200;
                }
                result.Data.Success.ToDepartments = new List<Desc>();
                return result;
            }

            if (!appInfo.AssignedDepartmentIds.Any())
            {
                result.Status = 99;
                result.Data.Error.ErrorDepartments = targetInfo.Departments.ConvertDepartmentIdToObject();
                return result;
            }

            var allAssignedDepartments = WeChatCommonService.GetSubDepartments(appInfo.AssignedDepartmentIds);

            var errorDepartments = targetInfo.Departments.Where(depart => allAssignedDepartments.All(x => x.id != depart)).ToList();

            if (errorDepartments.Any())
            {
                result.Status = 99;
            }
            else
            {
                if (result.Status == 0)
                {
                    result.Status = 200;
                }

                result.Data.Success.ToDepartments = targetInfo.Departments.ConvertDepartmentIdToObject();
                return result;
            }

            var successDepartments = targetInfo.Departments.Where(x => errorDepartments.All(y => x != y)).ToList();

            result.Data.Error.ErrorDepartments = errorDepartments.ConvertDepartmentIdToObject();

            result.Data.Success.ToDepartments = successDepartments.ConvertDepartmentIdToObject();

            return result;
        }
    }
}
