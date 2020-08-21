using Infrastructure.Core;
using DLYB.CA.Entity;
using System.Collections.Generic;

namespace DLYB.CA.Contracts
{
    public interface IQuestionImagesService : IDependency, IBaseService<QuestionImages>
    {
        List<T> GetListByQuestionID<T>(int QuestionId) where T : IViewModel, new();
    }
}
