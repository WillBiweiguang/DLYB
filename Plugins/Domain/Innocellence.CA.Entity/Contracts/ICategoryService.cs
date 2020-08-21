using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.CA.Entity;
namespace Innocellence.CA.Contracts
{
    public interface ICategoryService : IDependency, IBaseService<Category>
    {
        List<T> GetListByCode<T>(string code) where T : IViewModel, new();
    }
}
