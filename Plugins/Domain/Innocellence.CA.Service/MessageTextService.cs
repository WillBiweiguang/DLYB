// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 17:21</last-date>
// -----------------------------------------------------------------------

using System.Data.Entity;
using System.Threading.Tasks;
using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.Service.Common;
using DLYB.CA.Contracts;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;
using DLYB.CA.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DLYB.CA.Service;
using DLYB.CA.Service.Common;
using DLYB.CA.Service.Interface;
using DLYB.Weixin.QY.AdvancedAPIs.App;
using DLYB.Weixin.QY.AdvancedAPIs.MailList;
using CheckResult = DLYB.CA.Entity.CheckResult;

namespace DLYB.CA.Services
{
    /// <summary>
    /// 业务实现——文字消息模块
    /// </summary>
    public partial class MessageTextService : BaseService<MessageText>, IMessageTextService
    {
        private readonly Dictionary<int, GetTagMemberResult> _tagMemberdDictionary = new Dictionary<int, GetTagMemberResult>();
        private readonly IPushService _pushService;
        private readonly IPushHistoryService _pushHistoryService;

        public MessageTextService(IUnitOfWork unitOfWork,
            IArticleThumbsUpService articleThumbsUpService, 
            IPushService pushService, 
            IPushHistoryService pushHistoryService)
            : base("CAAdmin")
        {
            _pushService = pushService;
            _pushHistoryService = pushHistoryService;
        }

        /// <summary>
        /// InsertView
        /// </summary>
        /// <param name="objModalSrc"></param>
        /// <returns></returns>
        public override int InsertView<T>(T objModalSrc)
        {
            var objModal = (MessageTextView)(IViewModel)objModalSrc;
            var obj = objModal.MapTo<MessageText>();
            obj.RefEntity = objModal.EventPersonCategory;
            obj.Status = ConstData.STATUS_NEW;
            int iRet = Repository.Insert(obj);
            objModal.Id = obj.Id;
            return iRet;
        }

        public override int UpdateView<T>(T obj, List<string> lst) 
        {
            var objModal = (MessageTextView)(IViewModel)obj;
            var message = objModal.MapTo<MessageText>();
            message.RefEntity = objModal.EventPersonCategory;
            return Repository.Update(message, lst);
        }

        public List<T> GetList<T>(Expression<Func<MessageText, bool>> predicate) where T : IViewModel, new()
        {
            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
        }

        public override List<T> GetList<T>(Expression<Func<MessageText, bool>> func, PageCondition page)
        {
            //TODO: need to refactor for reducing the roundtrip between with db server
            var total = 0;

            var list = GetList<MessageTextView>(func, page.PageIndex, page.PageSize, ref total, page.SortConditions);
            var ids = list.AsParallel().Select(x => x.Id).ToList();
         
            page.RowCount = total;

            var histories = (from history in _pushHistoryService.Repository.Entities.Where(
                  x => ids.Contains(x.ContentId) && x.ContentType == ContentType.Text.ToString())
                             group history by history.ContentId
                                 into groups
                                 select groups.OrderByDescending(x => x.CreatedDate).Select(x => new { x.Id, x.ContentId }).FirstOrDefault()).ToList();

            Parallel.ForEach(list, x =>
            {
                var pushHistory = histories.FirstOrDefault(y => y.ContentId == x.Id);
                x.HistoryId = pushHistory == null ? 0 : pushHistory.Id;
            });

            return list.Select(x => (T)(IViewModel)x).ToList();
        }

        public Result<object> CheckMessagePushRule(int appId, IList<int> selectedDepartmentIds, IList<int> selectedTagIds, IList<string> selectedLillyIds, int contentId, bool isToAllUsers)
        {
            var result = _pushService.PushCheck(new TargetInfoObject
            {
                Departments = selectedDepartmentIds,
                IsToAllUsers = isToAllUsers,
                Tags = selectedTagIds,
                Users = selectedLillyIds,
                ContentId = contentId
            }, appId, ContentType.Text);

            return result;
        }

        private bool CheckDepartment(GetAppInfoResult appInfo, IList<int> selectedDepartmentIds, CheckResult checkResult)
        {
            //直接分配给应用
            var assignedDepartmentIds = appInfo.allow_partys.partyid.ToList();

            var underTagDepartmentIDs = _tagMemberdDictionary.Select(x => x.Value).SelectMany(x => x.partylist).ToList();

            assignedDepartmentIds.AddRange(underTagDepartmentIDs);

            //TODO:调整
            if (!assignedDepartmentIds.Any())
            {
                if (!selectedDepartmentIds.Any()) return true;

                checkResult.ErrorDepartments = GetErrorDepartments(selectedDepartmentIds);

                return false;
            }

            var departments = WeChatCommonService.GetSubDepartments(assignedDepartmentIds.Distinct().ToList()).ToList();

            #region hiddlen
            //var allowUsers = WeChatCommonService.lstUser.Where(x => x.department.Any(y => departments.Any(z => z.id == y))).ToList();

            //var needUsers = WeChatCommonService.lstUser.Where(x => x.department.Any(y => selectedDepartmentIds.Any(s => s == y))).ToList();

            //var errorUsers = needUsers.Where(x => allowUsers.All(u => x.userid != u.userid)).ToList();

            //if (!errorUsers.Any())
            //{
            //    return true;
            //}

            //var errorDepartments = WeChatCommonService.lstDepartment.Where(x => errorUsers.SelectMany(y => y.department).Any(z => z == x.id)).ToList();

            //var errorDepartmentIds = selectedDepartmentIds.Where(x => WeChatCommonService.GetSubDepartments(x).Any(y => errorDepartments.Any(z => z.id == y.id))).ToList();

            //if (!errorDepartmentIds.Any()) return true;
            #endregion

            var errorDepartmentIds = selectedDepartmentIds.Where(x => departments.All(y => x != y.id)).ToList();

            if (!errorDepartmentIds.Any())
            {
                return true;
            }

            checkResult.ErrorDepartments = GetErrorDepartments(errorDepartmentIds);

            return false;
        }

        private bool CheckTag(GetAppInfoResult appInfo, IList<int> selectedTagIds, IEnumerable<string> allAssignedUsers, CheckResult checkResult)
        {
            if (!selectedTagIds.Any()) return true;

            var needTags = selectedTagIds.Select(selectedTagId => new TagEntity { TagId = selectedTagId, TagMember = WeChatCommonService.GetTagMembers(selectedTagId, int.Parse(appInfo.agentid)) }).ToList();

            //needTags.Select(x => new { TagId = x.Key, TagMember = x.Value }).ToList();

            var needUsers = needTags.SelectMany(x => x.TagMember.userlist.Select(y => y.userid)).ToList();

            var needParties = WeChatCommonService.GetSubDepartments(needTags.SelectMany(x => x.TagMember.partylist).ToList()).ToList();

            needUsers.AddRange(WeChatCommonService.lstUser.Where(x => needParties.Any(y => x.department.Any(z => z == y.id))).Select(x => x.userid).ToList());

            var errorUsers = needUsers.Where(x => allAssignedUsers.Any(y => x == y)).ToList();

            var errorTags = needTags.Where(x => errorUsers.Any(y => x.TagMember.userlist.Any(z => z.userid == y))).Select(x => x.TagId).ToList();

            if (!errorTags.Any()) return true;

            checkResult.ErrorTags = GetErrorTags(errorTags);
            return false;
        }

        private static bool CheckUser(IEnumerable<string> allAssignedUsers, IList<string> selectedLillyIds, CheckResult checkResult)
        {
            if (!selectedLillyIds.Any())
            {
                return true;
            }

            var errorUsers = selectedLillyIds.Where(id => allAssignedUsers.All(assignedUserId => string.Compare(id, assignedUserId, StringComparison.CurrentCultureIgnoreCase) != 0) || WeChatCommonService.lstUser.First(x => string.Compare(x.userid, id, StringComparison.CurrentCultureIgnoreCase) == 0).status == 4).ToList();

            if (!errorUsers.Any()) return true;
            checkResult.ErrorUsers = errorUsers;
            return false;
        }

        private static IList<Desc> GetErrorDepartments(IEnumerable<int> errorDepartments)
        {
            return errorDepartments.Select(x =>
                {
                    var department = WeChatCommonService.lstDepartment.FirstOrDefault(y => x == y.id);
                    if (department == null)
                    {
                        throw new DLYBException(string.Format("the department id {0} have not been find!", x));
                    }
                    return new Desc { Id = department.id, Name = department.name };
                }).ToList();
        }

        private static IList<Desc> GetErrorTags(IEnumerable<int> errorTags)
        {
            return errorTags.Select(x =>
            {
                var tag = WeChatCommonService.lstTag.FirstOrDefault(y => int.Parse(y.tagid) == x);
                if (tag == null)
                {
                    throw new DLYBException(string.Format("the tag id {0} have not been find!", x));
                }
                return new Desc { Id = int.Parse(tag.tagid), Name = tag.tagname };
            }).ToList();
        }

        private bool CheckVisualRange(GetAppInfoResult appInfo, out  IList<string> allAssignedUsers, Action func = null)
        {
            allAssignedUsers = null;

            var isConfig = appInfo.allow_partys.partyid.Any() || appInfo.allow_tags.tagid.Any() || appInfo.allow_userinfos.user.Any();
            if (!isConfig)
            {
                return false;
            }

            if (func != null)
            {
                func();
            }

            //TODO:获取直接配置的用户信息
            var assignedUsers = appInfo.allow_userinfos.user.Select(x => x.userid).ToList();

            var departments = appInfo.allow_partys.partyid.ToList();

            foreach (var tagInfo in _tagMemberdDictionary.Values)
            {
                //user under tag
                assignedUsers.AddRange(tagInfo.userlist.Select(x => x.userid).ToList());

                departments.AddRange(tagInfo.partylist);
            }

            var subDepartments = WeChatCommonService.GetSubDepartments(departments.Distinct().ToList()).ToList();

            //TODO:获取部门下的人
            assignedUsers.AddRange(WeChatCommonService.lstUser.Where(x => x.department.Any(y => subDepartments.Any(d => d.id == y))).Select(x => x.userid).ToList());

            allAssignedUsers = assignedUsers.Distinct().ToList();

            return true;
        }

        private void GenerateTagInfoDictionary(GetAppInfoResult appInfo)
        {
            foreach (var allowTag in appInfo.allow_tags.tagid)
            {
                _tagMemberdDictionary.Add(allowTag, WeChatCommonService.GetTagMembers(allowTag, int.Parse(appInfo.agentid)));
            }
        }

        public PushHistoryEntity GetPushHistory(int historyId)
        {
            return _pushHistoryService.Repository.Entities.Where(x => x.Id == historyId && x.ContentType == ContentType.Text.ToString()).Include(x => x.Details).FirstOrDefault();
        }

        /// <summary>
        /// 实现接口 IPushStrategyService
        /// </summary>
        private readonly IList<IPushCheckRule> _rules = new List<IPushCheckRule> { new DefaultUserRule(), new DefaultDepartmentRule(), new DefaultTagRule() };

    }

}