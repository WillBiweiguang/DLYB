using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrastructure.Core;
using DLYB.CA.Entity;
using DLYB.CA.ModelsView;


namespace DLYB.CA.Contracts
{
    public interface IQuestionManageService : IDependency, IBaseService<QuestionManage>
    {
        List<T> GetListByQUserId<T>(int appid, string qUserId,string category) where T : IViewModel, new();
        List<T> GetQuestionList<T>(Expression<Func<QuestionManage, bool>> predicate) where T : IViewModel, new();
        QuestionManageView GetQuestionDetail(string id, QuestionManage obj);
        QuestionManageView GetFrontQuestionDetail(int id, QuestionManage obj);
        void UpdateStatus(QuestionManageView obj);
    }
}
