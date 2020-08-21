using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.CA.Entity;
namespace Innocellence.CA.Contracts
{
    public interface ISurveyService : IDependency, IBaseService<Survey>
    {
        List<T> GetListByCode<T>(string Surveycode) where T : IViewModel, new();
    }
}
