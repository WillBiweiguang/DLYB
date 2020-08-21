using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Core;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Entity;
using DLYB.CA.Service.Common;
using DLYB.Weixin.QY.AdvancedAPIs.App;
using DLYB.Weixin.QY.AdvancedAPIs.MailList;
using DLYB.Weixin.QY.AdvancedAPIs.Mass;

namespace DLYB.CA.Service.Interface
{
    public interface IPushService : IDependency
    {
        Result<object> PushCheck(TargetInfoObject entity, int appId, ContentType contentType);

        Task Update(MassResult messageResult, int historyId);
    }

    public class ErrorResult
    {
        public IList<string> ErrorUsers { get; set; }

        public IList<Desc> ErrorDepartments { get; set; }

        public IList<Desc> ErrorTags { get; set; }
    }

    public class SuccessResult
    {
        public IList<string> ToUsers { get; set; }

        public IList<Desc> ToDepartments { get; set; }

        public IList<Desc> ToTags { get; set; }
    }

    public class CheckResult
    {
        public ErrorResult Error { get; set; }

        public SuccessResult Success { get; set; }
    }

    public enum ContentType
    {
        [Description("news")]
        News,

        [Description("Message")]
        Message,

        [Description("text")]
        Text,

        [Description("voice")]
        Video
    }

    public class TargetInfoObject
    {
        public IList<int> Departments { get; set; }

        public IList<string> Users { get; set; }

        public IList<int> Tags { get; set; }

        public bool IsToAllUsers { get; set; }

        public int ContentId { get; set; }
    }

    public enum OperationType
    {
        Cancel,
        Continue
    }

    public enum ErrorType
    {
        Checked,

        Returned
    }

    public enum TargetType
    {
        AllUser,
        Config
    }

    public class ConfigedInfo
    {
        public List<int> AssignedDepartmentIds { get; set; }

        public List<string> AssignedUserIds { get; set; }

        public GetAppInfoResult AppInfoResult { get; private set; }

        public List<int> DepartmentsUnderSelectedTag { get; set; }

        public IDictionary<int, GetTagMemberResult> Dictionary { get; private set; }

        public ConfigedInfo(GetAppInfoResult appInfo)
        {
            Dictionary = new Dictionary<int, GetTagMemberResult>();
            AppInfoResult = appInfo;

            AssignedDepartmentIds = (appInfo.allow_partys == null || appInfo.allow_partys.partyid == null || !appInfo.allow_partys.partyid.Any()) ? new List<int>() : appInfo.allow_partys.partyid.ToList();

            AssignedUserIds = (appInfo.allow_userinfos == null || appInfo.allow_userinfos.user == null || !appInfo.allow_userinfos.user.Any()) ? new List<string>() : appInfo.allow_userinfos.user.Select(x => x.userid).ToList();

            //append depart and user of tag
            var tagIds = (appInfo.allow_tags == null || appInfo.allow_tags.tagid == null || !appInfo.allow_tags.tagid.Any()) ? new List<int>() : appInfo.allow_tags.tagid.ToList();

            foreach (var tagId in tagIds)
            {
                var memberResult = WeChatCommonService.GetTagMembers(tagId, int.Parse(appInfo.agentid));

                AssignedDepartmentIds.AddRange(memberResult.partylist);

                AssignedUserIds.AddRange(memberResult.userlist.Select(x => x.userid).ToList());

                Dictionary.Add(tagId, memberResult);
            }

            var subDepartments = WeChatCommonService.GetSubDepartments(AssignedDepartmentIds.Distinct().ToList()).ToList();

            AssignedUserIds.AddRange(WeChatCommonService.lstUser.Where(x => x.department.Any(y => subDepartments.Any(d => d.id == y))).Select(x => x.userid).ToList());

            AssignedUserIds = AssignedUserIds.Distinct().ToList();
        }
    }

    public enum StatusCode
    {
        CheckError = 1,
        Others = 99,
        Success = 200,
    }
}
