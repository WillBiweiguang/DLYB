using Infrastructure.Core;
using DLYB.CA.Entity;
using System.Collections.Generic;

namespace DLYB.CA.Contracts
{
    public interface IQuestionSubService : IDependency, IBaseService<QuestionSub>
    {
        List<T> GetListByQuestionID<T>(int questionId) where T : IViewModel, new();
        int GetSubId(int id);
    }
}
