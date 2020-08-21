using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Entity;
using DLYB.CA.Service.Common;

namespace DLYB.CA.Service.Interface
{
    public interface IPushStrategyService : IDependency
    {
        Result<CheckResult> CheckPushRule(object entity, ConfigedInfo appInfo);

        ContentType ContentType { get; }
    }

    public class BaseStrategyService
    {
        protected virtual void HandlerResult(Result<CheckResult> result, ConfigedInfo appInfo, object entity)
        {
            if (result.Status == 200)
                return;

            var errorDepartments = new List<Desc>();
            var errorTags = new List<Desc>();

            var targetInfo = (TargetInfoObject)entity;

            var selectedDepartmentIds = targetInfo.Departments;

            //1. find out department from error departments ,that does not select
            //2.
            var errorInfo = result.Data.Error;

            #region
            if (errorInfo.ErrorDepartments != null && errorInfo.ErrorDepartments.Any())
            {
                var errorDepartmentIdsUnderSelected = selectedDepartmentIds.Where(x =>
                {
                    var subDepartments = WeChatCommonService.GetSubDepartments(x);
                    return subDepartments.Any(y => errorInfo.ErrorDepartments.Any(z => y.id == z.Id));
                });

                errorDepartments.AddRange(errorDepartmentIdsUnderSelected.ConvertDepartmentIdToObject());

                var errorTagsFromDepartment = targetInfo.Tags.Where(tag =>
                 {
                     var departmentsUnderSelectedTag = appInfo.Dictionary[tag].partylist;

                     if (errorInfo.ErrorDepartments.Any(x => WeChatCommonService.GetSubDepartments(departmentsUnderSelectedTag).Any(y => x.Id == y.id)))
                     {
                         return true;
                     }
                     return false;
                 }).ToList();

                errorTags.AddRange(errorTagsFromDepartment.ConvertTagIdToObject());
            }
            #endregion

        }
    }

    public static class EnumerableExtension
    {
        public static IList<Desc> ConvertDepartmentIdToObject(this IEnumerable<int> departmentIds)
        {
            return departmentIds.Select(x =>
            {
                var department = WeChatCommonService.lstDepartment.FirstOrDefault(y => x == y.id);
                if (department == null)
                {
                    throw new DLYBException(string.Format("the department id {0} have not been find!", x));
                }
                return new Desc { Id = department.id, Name = department.name };
            }).ToList();
        }

        public static IList<Desc> ConvertTagIdToObject(this IEnumerable<int> tagIds)
        {
            return tagIds.Select(x =>
            {
                var tag = WeChatCommonService.lstTag.FirstOrDefault(y => x == int.Parse(y.tagid));
                if (tag == null)
                {
                    throw new DLYBException(string.Format("the tag id {0} have not been find in cache!", x));
                }
                return new Desc { Id = int.Parse(tag.tagid), Name = tag.tagname };
            }).ToList();
        }
    }
}
