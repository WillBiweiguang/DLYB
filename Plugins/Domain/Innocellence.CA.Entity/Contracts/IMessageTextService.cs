using System;
using System.Collections.Generic;
using Infrastructure.Core;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;
using System.Linq.Expressions;


namespace DLYB.CA.Contracts
{
    public interface IMessageTextService : IDependency, IBaseService<MessageText>
    {
        List<T> GetList<T>(Expression<Func<MessageText, bool>> predicate) where T : IViewModel, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="selectedDepartmentIds"></param>
        /// <param name="selectedTagIds"></param>
        /// <param name="selectedLillyIds"></param>
        /// <param name="checkResult"></param>
        /// <param name="contentId"></param>
        /// <param name="isToAllUsers">如果是全员发, 把selectedLillyIds参数传入全部人员id</param>
        /// <returns></returns>
        Result<object> CheckMessagePushRule(int appId, IList<int> selectedDepartmentIds, IList<int> selectedTagIds, IList<string> selectedLillyIds, int contentId, bool isToAllUsers = false);

        PushHistoryEntity GetPushHistory(int historyId);
    }
}
